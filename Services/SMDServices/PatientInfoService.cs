using Data.Constants;
using Data.DbContexts;
using Data.Entities.SMDEntities;
using Data.Models;
using Data.Models.SMDModels;
using Mapster;
using Services.Extenstions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Data.Models.CustomModels;
using Services.LookupServices;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.SMDServices
{
    public interface IPatientInfoService
    {
        public Task<PagingModel2> GetAsync(PatientInfoFilterModel model, CustomUser user);
        public Task<ResultModel> AddPatientInfoAsync(PatientInfoCreateModel model, CustomUser user);
        public Task<ResultModel> UpdatePatientInfoAsync(PatientInfoUpdateModel model, CustomUser user);
        public Task<ResultModel> DeletePatientInfoAsync(Guid id, CustomUser user);
        public Task AddOrUpdatePatientInfoAsync(IEnumerable<PatientInfo> entites, IEnumerable<Unit> validCBOs, CustomUser user, bool forceDelete = false);
        Task<ResultModel> GetHistoryAsync(Guid patientInfoId, int pageIndex = 0, int pageSize = int.MaxValue);
    }

    public class PatientInfoService : IPatientInfoService
    {
        private readonly AppDbContext _dbContext;
        private readonly IndicatorLookupService _indicatorLookup;

        public PatientInfoService(AppDbContext dbContext, IndicatorLookupService indicatorLookup)
        {
            _dbContext = dbContext;
            _indicatorLookup = indicatorLookup;
        }

        #region Update Report
        private async Task UpdateReport(IEnumerable<PatientInfo> entities, IEnumerable<Unit> validCBOs, IEnumerable<DateTime> dates, CustomUser user)
        {
            var cboIds = entities.Select(_ => _.CBOId).Distinct();
            // re-calculate report
            var indicators = _indicatorLookup.GetIntegerIndicators();
            var dTempIndicator = indicators.FirstOrDefault(_ => _.Code == "HTS_NEG_2");
            var effIndicators = _indicatorLookup.GetEfficiencyIndicators().ToList();
            var allInfos = _dbContext.PatientInfos.BaseFilter().Where(_ => cboIds.Contains(_.CBOId) && dates.Contains(_.ReportingPeriod)).ToList();
            if (allInfos == null || !allInfos.Any())
            {
                _dbContext.UpdateReportWithDeletedPatientInfo(user.Username, entities, indicators, dates);
                await _dbContext.SaveChangesAsync();
                return;
            }
            var temp = allInfos.GetReportIndicatorTempCollection(indicators);
            var tasks = new List<Task<IEnumerable<Report>>>();
            foreach (var indicator in indicators)
            {
                tasks.Add(temp.ToReportAsync(validCBOs, indicator));
            }
            var taskResult = await Task.WhenAll(tasks);
            var reports = taskResult.Where(x => x != null).SelectMany(x => x).ToList();
            var sum_dTemp = reports.Where(x => x.IndicatorId == dTempIndicator.Id).Sum(x => x.Value);
            _dbContext.UpdateReport(user.Username, reports, effIndicators, dates);
            await _dbContext.Reports.AddRangeAsync(reports);
            await _dbContext.SaveChangesAsync();
            // calculate efficiency indicator
            var paymentTemps = reports.Adapt<List<RecalByPaymentModel>>();
            await _dbContext.RecalIndicatorEffeciency(paymentTemps);
        }

        private IQueryable<PatientInfo> FilterByRole(CustomUser user, IQueryable<PatientInfo> filter)
        {
            switch (user.Role)
            {
                case Role.SMD_CBO:
                    var unit = _dbContext.Units.GetCBOByUsernameAsync(user.Username).Result;
                    return filter.Where(x => x.CBOId == unit.Id);
                case Role.SMD_PROJECT:
                    var units = _dbContext.Units.GetCBOsInProject(user.UnitId);
                    return filter.Where(x => units.Select(u => u.Id).Contains(x.CBOId));
                case Role.SMD_ADMIN:
                    return filter;
                default:
                    return Enumerable.Empty<PatientInfo>().AsQueryable();
            }
        }

        private async Task<List<DateTime>> AddOrUpdatePatienInfoAsync(IEnumerable<PatientInfo> entities, string username, bool forceDelete = false)
        {
            bool hasToChangeReportPeriod = false;
            var reachCodes = entities.Select(_ => _.ReachCode);
            var cboIds = entities.Select(_ => _.CBOId).Distinct();
            var reportPeriods = entities.Select(_ => _.ReportingPeriod).Distinct().ToList();
            if (forceDelete)
            {
                var patientToDeletes = _dbContext.PatientInfos.BaseFilter().Where(_ => cboIds.Contains(_.CBOId) && reportPeriods.Contains(_.ReportingPeriod));
                _dbContext.PatientInfos.RemoveRange(patientToDeletes);
                await _dbContext.SaveChangesAsync();
                _dbContext.PatientInfos.AddRange(entities);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // find all patient info of these Cbos and within these period in database (also used to re-calculate report of the cbos in these reporting period)
                var updateInfos = _dbContext.PatientInfos.BaseFilter().Where(_ => cboIds.Contains(_.CBOId) && reachCodes.Contains(_.ReachCode)).ToList();
                if (updateInfos != null) // 
                {
                    var originalReportPeriods = updateInfos.Select(_ => _.ReportingPeriod).Distinct().ToList();
                    // update old infos
                    foreach (var dup in updateInfos)
                    {
                        var entity = entities.FirstOrDefault(_ => _.CBOId == dup.CBOId && _.ReachCode == dup.ReachCode);
                        if (entity.ReportingPeriod != dup.ReportingPeriod || entity.IsDeleted)
                            hasToChangeReportPeriod = true;
                        _dbContext.UpdatePatientInfo(dup, entity, username);
                    }
                    if (hasToChangeReportPeriod)
                        reportPeriods.AddRange(originalReportPeriods);
                }
                var newInfos = entities.Except(updateInfos, new PatientInfoComparer());
                _dbContext.PatientInfos.AddRange(newInfos);
                await _dbContext.SaveChangesAsync();
            }

            return reportPeriods;
        }
        #endregion

        public async Task AddOrUpdatePatientInfoAsync(IEnumerable<PatientInfo> entities, IEnumerable<Unit> validCBOs, CustomUser user, bool forceDelete = false)
        {
            using (var trans = _dbContext.Database.BeginTransaction())
                try
                {
                    var cboIds = entities.Select(_ => _.CBOId).Distinct();
                    var reportPeriods = await AddOrUpdatePatienInfoAsync(entities, user.Username, forceDelete);
                    await UpdateReport(entities, validCBOs, reportPeriods, user);
                    trans.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
        }

        public async Task<ResultModel> AddPatientInfoAsync(PatientInfoCreateModel model, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var cbos = _dbContext.GetCBOsByRole(user);
                var entity = model.BuildAdapter().AddParameters("CBOs", cbos).AdaptToType<PatientInfo>();
                var list = new List<PatientInfo>() { entity };
                var reportPeriods = await AddOrUpdatePatienInfoAsync(list, user.Username);
                await UpdateReport(list, cbos, reportPeriods, user);
                result.Data = entity.Adapt<PatientInfoViewModel>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<PagingModel2> GetAsync(PatientInfoFilterModel model, CustomUser user)
        {
            var nullDate = new DateTime(1, 1, 1);
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.PatientInfos.BaseFilter().FilterPatientInfo(model);
                filter = this.FilterByRole(user, filter);
                result.PageCount = filter.PageCount(model.PageSize);
                var data = (await filter.PageData(model.PageIndex, model.PageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<PatientInfoViewModel>>()).ToList();
                data.ForEach(d =>
                {
                    if (d.UpdatedDate != null && d.UpdatedDate.Value.Year == 1)
                    {
                        d.UpdatedDate = null;
                    }
                });
                result.Data = data;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> UpdatePatientInfoAsync(PatientInfoUpdateModel model, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                model.FormatData();
                var cbos = _dbContext.GetCBOsByRole(user);
                var eModel = model.BuildAdapter().AddParameters("CBOs", cbos).AdaptToType<PatientInfo>();
                var list = new List<PatientInfo>() { eModel };
                var reportPeriods = await AddOrUpdatePatienInfoAsync(list, user.Username);
                await UpdateReport(list, cbos, reportPeriods, user);
                result.Data = eModel.Adapt<PatientInfoViewModel>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> DeletePatientInfoAsync(Guid id, CustomUser user)
        {
            var result = new ResultModel();
            using (var trans = _dbContext.Database.BeginTransaction())
                try
                {
                    var entity = _dbContext.PatientInfos.BaseFilter().AsNoTracking().FirstOrDefault(_ => _.Id == id);
                    if (entity == null)
                        throw new Exception(ErrorMessages.ID_NOT_FOUND);
                    entity.IsDeleted = true;
                    var list = new List<PatientInfo>() { entity };
                    var reportPeriods = await AddOrUpdatePatienInfoAsync(list, user.Username);
                    var cbos = _dbContext.GetCBOsByRole(user);
                    await UpdateReport(list, cbos, reportPeriods, user);
                    trans.Commit();
                    result.Succeed = true;
                }
                catch (Exception e)
                {
                    e.Adapt(result);
                }
            return result;
        }

        public async Task<ResultModel> GetHistoryAsync(Guid patientInfoId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.PatientInfoHistories.BaseFilter().Where(_ => _.PatientInfoId == patientInfoId);
                filter = filter.OrderByDescending(_ => _.DateUpdated);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<PatientInfoHistoryModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }
    }
}
