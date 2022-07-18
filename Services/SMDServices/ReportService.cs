using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Entities.SMDEntities;
using Data.Models;
using Data.Models.CustomModels;
using Data.Models.SMDModels;
using Ganss.Excel;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using Services.Extenstions;
using Services.LookupServices;
using Services.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.SMDServices
{
    public interface IReportService
    {
        Task<ResultModel> GetAsync(Guid? projectId, Guid? unitId, Guid? indicatorId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> GetHistoryAsync(Guid reportId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> GetAsync(ReportBarChartFilterModel model, CustomUser user);
        Task<ResultModel> GetExcelAsync(ReportGeneralFilterModel model, CustomUser user);
        Task<ResultModel> GetByCBOAsync(CustomUser user, Guid? indicatorId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> GetByProjectAsync(CustomUser user, Guid? unitId, Guid? indicatorId, int pageIndex = 0, int pageSize = int.MaxValue);
        ResultModel Summary(ReportGeneralFilterModel model, CustomUser user);
        ResultModel BarChart(ReportBarChartFilterModel model, CustomUser user);
        ResultModel Efficiency(ReportEfficiencyFilterModel model, CustomUser user);
        Task<ResultModel> CreateAsync(ReportCreateModel model, CustomUser user);
        Task<ResultModel> UpdateAsync(ReportUpdateModel model, CustomUser user);
        Task<ResultModel> DeleteAsync(Guid id);
        Task<ResultModel> ReadReportExcelAggregateForCBOAsync(IFormFile file, CustomUser user, ReadType readType);
        Task<ResultModel> ReadReportExcelAggregateForProjectAsync(IFormFile file, CustomUser user, ReadType readType);
        Task<ResultModel> ReadReportExcelIndividualForCBOAsync(IFormFile file, CustomUser user, ReadType readType, bool forceDelete = false);
        Task<ResultModel> ReadReportExcelIndividualForProjectAsync(IFormFile file, CustomUser user, ReadType readType, bool forceDelete = false);
        Task<ResultModel> ExposedAggregateForCBOAsync(ICollection<ReportAggregateModel> models, CustomUser user);
        Task<ResultModel> ExposedAggregateForProjectAsync(ICollection<ReportAggregateModel> models, CustomUser user);
        Task<ResultModel> ExposedIndividualForCBOAsync(ICollection<ReportIndividualModel> models, CustomUser user, bool forceDelete = false);
        Task<ResultModel> ExposedIndividualForProjectAsync(ICollection<ReportIndividualModel> models, CustomUser user, bool forceDelete = false);
        Task<ResultModel> ExposedPaymentForCBOAsync(ICollection<ReportPaymentModel> models, CustomUser user);
        Task<ResultModel> ExposedPaymentForProjectAsync(ICollection<ReportPaymentModel> models, CustomUser user);
        Task<ResultModel> SyncTarget();
        Task<ResultModel> ListProvinces(DateTime? from, DateTime? to, CustomUser user);
        Task<ResultModel> ListCBOs(DateTime? from, DateTime? to, CustomUser user);
        ResultModel GetLastUpdated();
    }

    public class ReportService : IReportService
    {
        private readonly AppDbContext _dbContext;
        private readonly IndicatorLookupService _indicatorLookup;
        private readonly PackageLookupService _packageLookup;
        private readonly IPatientInfoService _patientInfoService;
        private readonly string PAYMENT_SHEET_NAME = "Payment";
        private readonly string INDIVIDUAL_SHEET_NAME = "Case Reporting";
        private readonly string AGGREGATE_SHEET_NAME = "Synthesis";

        public ReportService(AppDbContext dbContext, IndicatorLookupService indicatorLookup, PackageLookupService packageLookup, IPatientInfoService patientInfoService)
        {
            _dbContext = dbContext;
            _indicatorLookup = indicatorLookup;
            _packageLookup = packageLookup;
            _patientInfoService = patientInfoService;
        }

        #region Report calculated by indicator 
        private async Task<IEnumerable<Report>> HTS_POS(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "HTS_POS_1";
                var resultModels = tempCollection.HTS_POS;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> CODE_2(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "ARV__1";
                var resultModels = tempCollection.ARVTransport;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> CODE_3_Percentage(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run((Func<IEnumerable<Report>>)(() =>
            {
                var indicatorCode = "RATE_ARV__1";
                var positiveResults = tempCollection.HTS_POS;
                if (positiveResults == null || positiveResults.Count() <= 0)
                    return null;
                var arvTransports = tempCollection.ARVTransport;
                if (arvTransports == null || arvTransports.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = positiveResults.GroupBy(_ => new { _.CBOName, _.ReportingPeriod });
                var result = Enumerable.Select(groupByPeriod, _ => (Report)new Report
                {
                    IndicatorId = indicator.Id,
                    Value = UtilityMethods.Percentage((int)_.FilterARVTransport().Count(), (int)_.Count()),
                    ValueType = ReportValueType.PERCENTAGE,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault((Func<Unit, bool>)(cbo => (bool)(cbo.Name == _.Key.CBOName))).Id,
                    CreatedMethod = CreatedMethod.IMPORT
                });
                return (IEnumerable<Report>)result;
            }));
        }

        private async Task<IEnumerable<Report>> CODE_4(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "TX_NEW__1";
                var resultModels = tempCollection.TX_New;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> CODE_5_Percentage(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run((Func<IEnumerable<Report>>)(() =>
            {
                var indicatorCode = "RATE_TX_NEW__1";
                var resultModels = tempCollection.ARVTransport;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = Enumerable.Select(groupByPeriod, _ => (Report)new Report
                {
                    IndicatorId = indicator.Id,
                    Value = UtilityMethods.Percentage((int)_.FilterTXNew().Count(), (int)_.Count()),
                    ValueType = ReportValueType.PERCENTAGE,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault((Func<Unit, bool>)(cbo => (bool)(cbo.Name == _.Key.CBOName))).Id,
                    CreatedMethod = CreatedMethod.IMPORT
                });
                return (IEnumerable<Report>)result;
            }));
        }

        private async Task<IEnumerable<Report>> CODE_6(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "PENDING_1";
                var resultModels = tempCollection.ARVPending;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> HTS_NEG(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "HTS_NEG_2";
                var resultModels = tempCollection.HTS_NEG;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> CODE_7(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "PREP__2";
                var resultModels = tempCollection.PrEPTransport;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> CODE_8_Percentage(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run((Func<IEnumerable<Report>>)(() =>
            {
                var indicatorCode = "RATE_PREP__2";
                var resultModels = tempCollection.HTS_NEG;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = Enumerable.Select(groupByPeriod, _ => (Report)new Report
                {
                    IndicatorId = indicator.Id,
                    Value = UtilityMethods.Percentage((int)_.FilterPrEPTransport().Count(), (int)_.Count()),
                    ValueType = ReportValueType.PERCENTAGE,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault((Func<Unit, bool>)(cbo => (bool)(cbo.Name == _.Key.CBOName))).Id,
                    CreatedMethod = CreatedMethod.IMPORT
                });
                return (IEnumerable<Report>)result;
            }));
        }

        private async Task<IEnumerable<Report>> CODE_9(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "PREP_NEW__2";
                var resultModels = tempCollection.PrEP_New;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }

        private async Task<IEnumerable<Report>> CODE_10_Percentage(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run((Func<IEnumerable<Report>>)(() =>
            {
                var indicatorCode = "RATE_PREP_NEW__2";
                var resultModels = tempCollection.PrEPTransport;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = Enumerable.Select(groupByPeriod, _ => (Report)new Report
                {
                    IndicatorId = indicator.Id,
                    Value = UtilityMethods.Percentage((int)_.FilterPrEPNew().Count(), (int)_.Count()),
                    ValueType = ReportValueType.PERCENTAGE,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault((Func<Unit, bool>)(cbo => (bool)(cbo.Name == _.Key.CBOName))).Id,
                    CreatedMethod = CreatedMethod.IMPORT
                });
                return (IEnumerable<Report>)result;
            }));
        }

        private async Task<IEnumerable<Report>> CODE_11(ReportIndicatorTempCollection tempCollection, IEnumerable<Unit> cbos)
        {
            return await Task.Run(() =>
            {
                var indicatorCode = "PENDING_2";
                var resultModels = tempCollection.PrEPPending;
                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var indicator = _indicatorLookup.Lookup(indicatorCode);
                if (indicator == null)
                    throw new Exception($"Indicator ({indicatorCode}) not found.");
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Name == _.Key.CBOName).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU
                });
                return result;
            });
        }
        #endregion

        #region Utility methods
        private async Task ReadReportExcelBatch(IFormFile file, Task<List<Unit>> getCBOsTask, ReadType readType, CustomUser user, bool forceDelete = false)
        {
            IWorkbook wb = await file.ReadAsWorkbookAsync();
            var cbos = await getCBOsTask;
            if (readType == ReadType.ALL || readType == ReadType.NORMAL_ONLY)
            {
                ISheet sh = wb.GetSheet(INDIVIDUAL_SHEET_NAME);
                if (sh == null)
                {
                    throw new Exception($"Sheet name '{INDIVIDUAL_SHEET_NAME}' not found.");
                }
                var models = await sh.ReadAsReportIndividualModels();
                await HandleImportNormalV2(models, cbos, user, forceDelete);
            }
            if (readType == ReadType.ALL || readType == ReadType.PAYMENT_ONLY)
            {
                ISheet sh = wb.GetSheet(PAYMENT_SHEET_NAME);
                if (sh == null)
                {
                    throw new Exception($"Sheet name '{PAYMENT_SHEET_NAME}' not found.");
                }
                var models = await sh.ReadAsReportPaymentModels();
                await HandleImportPayment(models, cbos, user);
            }
        }

        private async Task<object> HandleImportNormalV2(ICollection<ReportIndividualModel> models, List<Unit> cbos, CustomUser user, bool forceDelete = false)
        {
            var validCBOs = CheckValidCBOsImported(models, cbos);
            var patientInfos = models.BuildAdapter().AddParameters("CBOs", cbos).AdaptToType<ICollection<PatientInfo>>();
            await _patientInfoService.AddOrUpdatePatientInfoAsync(patientInfos, validCBOs, user, forceDelete);
            return null;
        }

        private async Task<object> HandleImportPayment(ICollection<ReportPaymentModel> models, List<Unit> cbos, CustomUser user)
        {
            CheckValidCBOsImported(models, cbos);
            CheckValidPaymentsImported(models);
            var reports = models.BuildAdapter().AddParameters("CBOs", cbos).AdaptToType<ICollection<Report>>();
            var ipackages = await _dbContext.ImplementPackages.BaseFilter().ToListAsync();
            var tempList = reports.ToList();
            UpdateReport(user.Username, tempList, ipackages: ipackages);
            if (tempList.Count > 0)
            {
                foreach (var report in tempList)
                {
                    report.AddTargetAmountToReport(ipackages);
                }
                _dbContext.Reports.AddRange(tempList);
            }
            await _dbContext.SaveChangesAsync();
            // calculate efficiency indicator
            var paymentTemps = reports.Adapt<List<RecalByPaymentModel>>();
            await _dbContext.RecalIndicatorEffeciency(paymentTemps);

            return reports;
        }

        private async Task ReadReportExcelAggregate(IFormFile file, Task<List<Unit>> getCBOsTask, ReadType readType, CustomUser user, bool forceDelete = false)
        {
            IWorkbook wb = await file.ReadAsWorkbookAsync();
            var cbos = await getCBOsTask;
            if (readType == ReadType.ALL || readType == ReadType.NORMAL_ONLY)
            {
                ISheet sh = wb.GetSheet(AGGREGATE_SHEET_NAME);
                if (sh == null)
                {
                    throw new Exception($"Sheet name '{AGGREGATE_SHEET_NAME}' not found.");
                }
                var models = await sh.ReadAsReportAggregateModels();
                await HandleImportAggregate(models, cbos, user.Username);
            }
            if (readType == ReadType.ALL || readType == ReadType.PAYMENT_ONLY)
            {
                ISheet sh = wb.GetSheet(PAYMENT_SHEET_NAME);
                if (sh == null)
                {
                    throw new Exception($"Sheet name '{PAYMENT_SHEET_NAME}' not found.");
                }
                var models = await sh.ReadAsReportPaymentModels();
                await HandleImportPayment(models, cbos, user);
            }
        }

        private async Task<object> HandleImportAggregate(ICollection<ReportAggregateModel> models, List<Unit> cbos, string username, bool forceDelete = false)
        {
            CheckValidCBOsImported(models, cbos);
            CheckValidIndicatorsImported(models);
            var reports = models.BuildAdapter().AddParameters("CBOs", cbos).AdaptToType<ICollection<Report>>();
            UpdateReport(username, reports);
            if (reports != null && reports.Count > 0)
            {
                _dbContext.Reports.AddRange(reports);
            }
            var count = await _dbContext.SaveChangesAsync();
            var paymentTemps = reports.Adapt<List<RecalByPaymentModel>>();
            await _dbContext.RecalIndicatorEffeciency(paymentTemps);
            return count;
        }

        private void UpdateReport(string username, ICollection<Report> reports, IEnumerable<ImplementPackage> ipackages = null)
        {
            var efficiencyIndicators = _indicatorLookup.GetEfficiencyIndicators().Select(_ => _.Id);
            var cbos = reports.Select(_ => _.UnitId);
            var indicators = reports.Select(_ => _.IndicatorId);
            var dates = reports.Select(_ => _.DateTime);

            var oldReports = _dbContext.Reports.BaseFilter().FilterDupeReportForImport(cbos, indicators, dates).ToList();
            if (oldReports != null)
            {
                foreach (var oldReport in oldReports)
                {
                    var newReport = reports.FirstOrDefault(_ => _.UnitId == oldReport.UnitId
                                                                && _.IndicatorId == oldReport.IndicatorId
                                                                && _.DateTime == oldReport.DateTime);
                    if (newReport == null)
                    {
                        continue;
                    }
                    if (efficiencyIndicators.Contains(oldReport.IndicatorId) && oldReport.TargetValue == null)
                    {
                        if (newReport.ValueType == ReportValueType.MONEY)
                        {
                            newReport.AddTargetAmountToReport(ipackages);
                        }
                    }
                    _dbContext.UpdateReport(oldReport, newReport, username);
                    reports.Remove(newReport);
                }
            }
        }

        private IEnumerable<Unit> CheckValidCBOsImported(IEnumerable<ReportBaseModel> models, IEnumerable<Unit> cbos)
        {
            if (HasFalseCBO(models, cbos))
            {
                string failLog = "";
                foreach (var model in models)
                {
                    var cboName = model.CBOName.ToUpper();
                    if (!cbos.Any(s => s.Name.ToUpper() == cboName || s.Code.ToUpper() == cboName))
                    {
                        failLog += $"Row: {model.Row} - " + ErrorMessages.NOT_ASSOCIATED_CBO + model.CBOName + Environment.NewLine;
                    }
                }
                if (!string.IsNullOrEmpty(failLog))
                    throw new Exception(failLog);
            }
            foreach (var model in models)
            {
                var cbo = cbos.FirstOrDefault(_ => _.Name == model.CBOName);
                if (cbo != null)
                    yield return cbo;
            }
        }

        private void CheckValidPaymentsImported(IEnumerable<ReportPaymentModel> models)
        {
            var groupBy = models.GroupBy(_ => new { _.CBOName, _.PSNU, _.ReportingPeriod });
            var dups = groupBy.SelectMany(_ => _.Skip(1));
            if (dups != null && dups.Any())
                throw new Exception(ErrorMessages.DUPLICATE_DATA + " : \n" + String.Join("\n", dups.Select(_ => $"{_.PSNU}, {_.CBOName}, {_.ReportingPeriod.Month}/{_.ReportingPeriod.Year}")));
        }

        private bool HasFalseCBO(IEnumerable<ReportBaseModel> models, IEnumerable<Unit> cbos)
        {
            return models.Any(_ => !cbos.Select(c => c.Name).Contains(_.CBOName));
        }

        private IEnumerable<T> GetFalseCBOs<T>(IEnumerable<T> models, IEnumerable<Unit> cbos) where T : ReportBaseModel
        {
            return models.Where(_ => !cbos.Select(c => c.Name).Contains(_.CBOName));
        }

        private void CheckValidCBOsImported(IEnumerable<ReportAggregateModel> models, IEnumerable<Unit> cbos)
        {
            if (HasFalseCBO(models, cbos))
            {
                string failLog = "";
                foreach (var model in models)
                {
                    var cboCode = model.CBOCode.ToUpper();
                    if (!cbos.Any(s => s.Name.ToUpper() == cboCode || s.Code.ToUpper() == cboCode))
                    {
                        failLog += $"Row: {model.Row} - " + ErrorMessages.NOT_ASSOCIATED_CBO + model.CBOCode + Environment.NewLine;
                    }
                }
                if (!string.IsNullOrEmpty(failLog))
                    throw new Exception(failLog);
            }
        }

        private bool HasFalseCBO(IEnumerable<ReportAggregateModel> models, IEnumerable<Unit> cbos)
        {
            return models.Any(_ => !cbos.Select(c => c.Code).Contains(_.CBOCode));
        }

        private IEnumerable<ReportAggregateModel> GetFalseCBOs(IEnumerable<ReportAggregateModel> models, IEnumerable<Unit> cbos)
        {
            return models.Where(_ => !cbos.Select(c => c.Code).Contains(_.CBOCode));
        }

        /// <summary>
        /// For Report excel single model only
        /// </summary>
        /// <param name="models"></param>
        /// <param name="cbos"></param>
        /// <exception cref="Exception"></exception>
        private void CheckValidIndicatorsImported(IEnumerable<ReportAggregateModel> models)
        {
            string failLog = "";
            foreach (var model in models)
            {
                var indicator = _indicatorLookup.Lookup(model.IndicatorCode);
                if (indicator == null || indicator.Type != ReportValueType.INTEGER)
                {
                    failLog += $"Row: {model.Row} - " + ErrorMessages.INVALID_IMPORT_INDICATOR + model.IndicatorCode + Environment.NewLine;
                }
            }
            if (!string.IsNullOrEmpty(failLog))
                throw new Exception(failLog);
        }

        private void CheckAllowInput(CustomUser user, AllowInputType allowInput)
        {
            switch (user.Role)
            {
                case Role.SMD_CBO:
                    var unit = _dbContext.Units.GetCBOByUsernameAsync(user.Username).Result;
                    if (unit == null)
                        throw new Exception(ErrorMessages.CBO_NOT_FOUND);
                    if (unit.AllowInputType != allowInput)
                        throw new Exception(ErrorMessages.INPUT_NOT_ALLOW);
                    break;
                case Role.SMD_PROJECT:
                    var project = _dbContext.Projects.Find(user.UnitId);
                    if (project == null)
                        throw new Exception(ErrorMessages.CBO_NOT_FOUND);
                    if (project.AllowInputType != allowInput)
                        throw new Exception(ErrorMessages.INPUT_NOT_ALLOW);
                    break;
                case Role.SMD_ADMIN:
                    throw new Exception(ErrorMessages.ROLE_NOT_SUITABLE);
                default:
                    throw new Exception(ErrorMessages.ROLE_NOT_SUITABLE);
            }
        }

        private IQueryable<Report> FilterByRole(CustomUser user, IQueryable<Report> filter)
        {
            switch (user.Role)
            {
                case Role.SMD_CBO:
                    var unit = _dbContext.Units.GetCBOByUsernameAsync(user.Username).Result;
                    return filter.FilterReport(null, unit.Id, null);
                case Role.SMD_PROJECT:
                    var project = _dbContext.Projects.FindById(user.UnitId);
                    return filter.FilterReport(user.UnitId, null, null);
                case Role.SMD_ADMIN:
                    return filter;
                default:
                    return Enumerable.Empty<Report>().AsQueryable();
            }
        }
        #endregion

        public async Task<ResultModel> CreateAsync(ReportCreateModel model, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<Report>();
                if (entity.ValueType == ReportValueType.INTEGER)
                    this.CheckAllowInput(user, AllowInputType.AGRREGATE);
                var dupe = _dbContext.Reports.FindDupe(entity.UnitId, entity.IndicatorId, entity.DateTime, entity.Period, entity.Province);
                if (dupe == null)
                {
                    entity.CreatedMethod = CreatedMethod.NORMAL;
                    if (entity.ValueType == ReportValueType.MONEY)
                    {
                        var ipackages = _dbContext.ImplementPackages.BaseFilter().ToList();
                        entity.AddTargetAmountToReport(ipackages);
                    }
                    _dbContext.Reports.Add(entity);
                }
                else
                {
                    entity.Adapt(dupe);
                    _dbContext.UpdateReport(dupe, entity, user.Username);
                }
                await _dbContext.SaveChangesAsync();
                var paymentTemps = new List<RecalByPaymentModel> { entity.Adapt<RecalByPaymentModel>() };
                await _dbContext.RecalIndicatorEffeciency(paymentTemps);
                result.Data = entity.Adapt<ReportViewModel>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> DeleteAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Reports.Find(id);
                entity.IsDeleted = true;
                _dbContext.Reports.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetAsync(Guid? projectId, Guid? unitId, Guid? indicatorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReport(projectId, unitId, indicatorId);
                filter = filter.Include(_ => _.Unit);
                filter = filter.OrderByDescending(_ => _.DateTime);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ReportViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetAsync(ReportBarChartFilterModel model, CustomUser user)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReportModel(model);
                filter = this.FilterByRole(user, filter);
                filter = filter.Include(_ => _.Unit);
                filter = filter.OrderByDescending(_ => _.DateTime).ThenBy(_ => _.IndicatorId);
                result.PageCount = filter.PageCount(model.PageSize);
                result.Data = await filter.PageData(model.PageIndex, model.PageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ReportViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Summary(ReportGeneralFilterModel model, Data.Entities.SMDEntities.CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReportModel(model);
                filter = this.FilterByRole(user, filter);

                var groupBy = filter.ToList().GroupBy(x => new { IndicatorId = x.IndicatorId, ValueType = x.ValueType });
                var integerIndicators = _indicatorLookup.GetIntegerIndicators();
                var integerReports = groupBy.CalculateAggregateValue(integerIndicators);
                var percentageIndicators = _indicatorLookup.GetPercentageIndicators();
                var percentageReport = integerReports.CalculateAggregatePercentageValue(percentageIndicators);
                var paymentIndicator = _indicatorLookup.GetPaymentIndicator();
                var paymentReport = groupBy.CalculateAggregatePaymentValue(paymentIndicator);
                result.Data = integerReports.Concat(percentageReport).Concat(paymentReport);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel BarChart(ReportBarChartFilterModel model, Data.Entities.SMDEntities.CustomUser user)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReportModel(model);
                filter = this.FilterByRole(user, filter);
                if (model.GroupByType == ReportGroupByType.PROJECTOR)
                {
                    filter = filter.Include(_ => _.Unit)
                                    .ThenInclude(_ => _.Project);
                    var names = filter.Select(x => x.Unit.Project.Name).Distinct().ToList();
                    result.PageCount = names.PageCount(model.PageSize);
                    var queryNames = names.PageData(model.PageIndex, model.PageSize);
                    var groupBy = filter.AsNoTracking()
                                        .ToList()
                                        .Where(x => queryNames.Contains(x.Unit.Project.Name))
                                        .GroupBy(x => new { IndicatorId = x.IndicatorId, GroupByType = x.Unit.Project.Name, ValueType = x.ValueType });
                    groupBy = groupBy.OrderBy(x => x.Key.GroupByType);
                    result.Data = groupBy.CalculateBarChartValue(model.Indicators);
                }
                else if (model.GroupByType == ReportGroupByType.TIME)
                {
                    var names = filter.Select(x => x.DateTime.ToString("MM/yyyy")).Distinct().ToList().OrderByDescending(s => $"{s.Split('/')[1]}{s.Split('/')[0]}");
                    var count = names.Count();
                    result.PageCount = names.PageCount(model.PageSize);
                    var queryNames = names.PageData(model.PageIndex, model.PageSize);
                    var groupBy = filter.AsNoTracking()
                                        .ToList()
                                        .Where(x => queryNames.Contains(x.DateTime.ToString("MM/yyyy")))
                                        .GroupBy(x => new { IndicatorId = x.IndicatorId, GroupByType = x.DateTime.ToString("MM/yyyy"), ValueType = x.ValueType, Date = x.DateTime });
                    groupBy = groupBy.OrderBy(s => $"{s.Key.GroupByType.Split('/')[1]}{s.Key.GroupByType.Split('/')[0]}"); ;
                    result.Data = groupBy.CalculateBarChartValue(model.Indicators);
                }
                else if (model.GroupByType == ReportGroupByType.PROVINCE)
                {
                    filter = filter.Include(_ => _.Unit);
                    var names = filter.Select(x => x.Unit.Province).Distinct().ToList();
                    result.PageCount = names.PageCount(model.PageSize);
                    var queryNames = names.PageData(model.PageIndex, model.PageSize);
                    var groupBy = filter.AsNoTracking()
                                        .Where(x => queryNames.Contains(x.Unit.Province))
                                        .ToList()
                                        .GroupBy(x => new { IndicatorId = x.IndicatorId, GroupByType = x.Unit.Province, ValueType = x.ValueType });
                    groupBy = groupBy.OrderBy(x => x.Key.GroupByType);
                    result.Data = groupBy.CalculateBarChartValue(model.Indicators);
                }
                else if (model.GroupByType == ReportGroupByType.CBO)
                {
                    filter = filter.Include(_ => _.Unit);
                    var names = filter.Select(x => x.Unit.Name).Distinct().ToList();
                    result.PageCount = names.PageCount(model.PageSize);
                    var queryNames = names.PageData(model.PageIndex, model.PageSize);
                    var groupBy = filter.AsNoTracking()
                                        .Where(x => queryNames.Contains(x.Unit.Name))
                                        .ToList()
                                        .GroupBy(x => new { IndicatorId = x.IndicatorId, GroupByType = x.Unit.Name, ValueType = x.ValueType });
                    groupBy = groupBy.OrderBy(x => x.Key.GroupByType);
                    result.Data = groupBy.CalculateBarChartValue(model.Indicators);
                }
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Efficiency(ReportEfficiencyFilterModel model, CustomUser user)
        {
            var result = new PagingModel2();
            try
            {
                var efficiencyIndicators = _indicatorLookup.GetEfficiencyIndicators().Select(_ => _.Id);
                var filter = _dbContext.Reports.BaseFilter().FilterReportModel(model, efficiencyIndicators);
                filter = this.FilterByRole(user, filter);
                filter = filter.Include(_ => _.Unit)
                                    .ThenInclude(_ => _.Project);
                var list = filter.AsNoTracking()
                                    .ToList()
                                    .OrderBy(_ => _.Unit.Project.Name)
                                    .ThenBy(_ => _.Unit.Name)
                                    .ThenBy(_ => _.DateTime)
                                    .ToList()
                                    ;
                result.Data = list.Adapt<IEnumerable<ReportEfficiencyModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetByCBOAsync(CustomUser user, Guid? indicatorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var unit = await _dbContext.Units.GetCBOByUsernameAsync(user.Username);
                var filter = _dbContext.Reports.BaseFilter().FilterReport(null, unit.Id, indicatorId);
                filter = filter.Include(_ => _.Unit);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ReportViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetByProjectAsync(CustomUser user, Guid? unitId, Guid? indicatorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var project = _dbContext.Projects.FindById(user.UnitId);
                var filter = _dbContext.Reports.BaseFilter().FilterReport(project.Id, unitId, indicatorId);
                filter = filter.Include(_ => _.Unit);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ReportViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> UpdateAsync(ReportUpdateModel model, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Reports.Find(model.Id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                var newEntity = model.Adapt<Report>();
                if (entity.ValueType == ReportValueType.INTEGER)
                    this.CheckAllowInput(user, AllowInputType.AGRREGATE);
                _dbContext.UpdateReport(entity, newEntity, user.Username);
                await _dbContext.SaveChangesAsync();
                var paymentTemps = new List<RecalByPaymentModel> { entity.Adapt<RecalByPaymentModel>() };
                await _dbContext.RecalIndicatorEffeciency(paymentTemps);
                result.Succeed = true;
                result.Data = entity.Adapt<ReportViewModel>();
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ReadReportExcelAggregateForCBOAsync(IFormFile file, CustomUser user, ReadType readType)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.AGRREGATE);
                var getCBOsTask = _dbContext.Units.GetCBOsByUsernameAsync(user.Username).AsNoTracking().ToListAsync();
                await ReadReportExcelAggregate(file, getCBOsTask, readType, user);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ReadReportExcelAggregateForProjectAsync(IFormFile file, CustomUser user, ReadType readType)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.AGRREGATE);
                var getCBOsTask = _dbContext.Units.GetCBOsInProject(user.UnitId).AsNoTracking().ToListAsync();
                await ReadReportExcelAggregate(file, getCBOsTask, readType, user);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ReadReportExcelIndividualForCBOAsync(IFormFile file, CustomUser user, ReadType readType, bool forceDelete = false)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.INDIVIDUAL);
                var getCBOsTask = _dbContext.Units.GetCBOsByUsernameAsync(user.Username).AsNoTracking().ToListAsync();
                await ReadReportExcelBatch(file, getCBOsTask, readType, user, forceDelete);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ReadReportExcelIndividualForProjectAsync(IFormFile file, CustomUser user, ReadType readType, bool forceDelete = false)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.INDIVIDUAL);
                var getCBOsTask = _dbContext.Units.GetCBOsInProject(user.UnitId).AsNoTracking().ToListAsync();
                await ReadReportExcelBatch(file, getCBOsTask, readType, user, forceDelete);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ExposedAggregateForCBOAsync(ICollection<ReportAggregateModel> models, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.AGRREGATE);
                var getCBOsTask = _dbContext.Units.GetCBOsByUsernameAsync(user.Username).AsNoTracking().ToListAsync();
                var cbos = await getCBOsTask;
                result.Data = await HandleImportAggregate(models, cbos, user.Username);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ExposedAggregateForProjectAsync(ICollection<ReportAggregateModel> models, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.AGRREGATE);
                var getCBOsTask = _dbContext.Units.GetCBOsInProject(user.UnitId).AsNoTracking().ToListAsync();
                var cbos = await getCBOsTask;
                result.Data = await HandleImportAggregate(models, cbos, user.Username);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ExposedIndividualForCBOAsync(ICollection<ReportIndividualModel> models, CustomUser user, bool forceDelete = false)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.INDIVIDUAL);
                var getCBOsTask = _dbContext.Units.GetCBOsByUsernameAsync(user.Username).AsNoTracking().ToListAsync();
                var cbos = await getCBOsTask;
                result.Data = await HandleImportNormalV2(models, cbos, user, forceDelete);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ExposedIndividualForProjectAsync(ICollection<ReportIndividualModel> models, CustomUser user, bool forceDelete = false)
        {
            var result = new ResultModel();
            try
            {
                this.CheckAllowInput(user, AllowInputType.INDIVIDUAL);
                var getCBOsTask = _dbContext.Units.GetCBOsInProject(user.UnitId).AsNoTracking().ToListAsync();
                var cbos = await getCBOsTask;
                result.Data = await HandleImportNormalV2(models, cbos, user, forceDelete);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ExposedPaymentForCBOAsync(ICollection<ReportPaymentModel> models, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var getCBOsTask = _dbContext.Units.GetCBOsByUsernameAsync(user.Username).AsNoTracking().ToListAsync();
                var cbos = await getCBOsTask;
                result.Data = await HandleImportPayment(models, cbos, user);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ExposedPaymentForProjectAsync(ICollection<ReportPaymentModel> models, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var getCBOsTask = _dbContext.Units.GetCBOsInProject(user.UnitId).AsNoTracking().ToListAsync();
                var cbos = await getCBOsTask;
                result.Data = await HandleImportPayment(models, cbos, user);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> SyncTarget()
        {
            var result = new ResultModel();
            try
            {
                var effIndicators = _indicatorLookup.GetEfficiencyIndicators().Select(_ => _.Id);
                var reports = await _dbContext.Reports.BaseFilter().Where(_ => effIndicators.Contains(_.IndicatorId) && !_.TargetValue.HasValue).ToListAsync();
                var targets = await _dbContext.Targets.BaseFilter().ToListAsync();
                var ipackages = await _dbContext.ImplementPackages.BaseFilter().ToListAsync();
                foreach (var item in reports)
                {
                    if (item.ValueType == ReportValueType.MONEY)
                    {
                        item.AddTargetAmountToReport(ipackages);
                    }
                }
                var count = await _dbContext.SaveChangesAsync();
                result.Data = count;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetHistoryAsync(Guid reportId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.ReportHistories.BaseFilter().Where(_ => _.ReportId == reportId);
                filter = filter.OrderByDescending(_ => _.DateUpdated);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ReportHistoryViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetExcelAsync(ReportGeneralFilterModel model, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReportModel(model);
                filter = this.FilterByRole(user, filter);
                filter = filter.Include(_ => _.Unit).ThenInclude(_ => _.Project)
                                .Include(_ => _.Indicator)
                                ;
                filter = filter.OrderByDescending(_ => _.DateTime).ThenBy(_ => _.IndicatorId);
                var data = filter.AsNoTracking().Adapt<IEnumerable<ReportExportExcelModel>>();
                using (NPOIMemoryStream ms = new NPOIMemoryStream())
                {
                    ms.AllowClose = false;
                    var em = new ExcelMapper();
                    em.Save(ms, data, sheetIndex: 0);
                    // reset position
                    ms.Position = 0;
                    // styling
                    IWorkbook workbook = new XSSFWorkbook(ms);
                    ISheet sheet = workbook.GetSheetAt(0);
                    // header
                    var row = sheet.GetRow(0);
                    row.SetRowNoneNullCellStyle(workbook.Bold(14));
                    for (int i = 0; i < 7; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                    using (var ms1 = new MemoryStream())
                    {
                        workbook.Write(ms1);
                        result.Data = ms1.ToArray();
                        result.Succeed = true;
                    }
                }
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel GetLastUpdated()
        {
            var rs = new ResultModel();
            try
            {
                rs.Succeed = true;
                var date1 = _dbContext.Reports.OrderByDescending(_ => _.DateTime).FirstOrDefault();
                rs.Data = new { LastUpdatedDate = date1.DateCreated };
            }
            catch (Exception e)
            {
                e.Adapt(rs);
            }
            return rs;
        }

        public async Task<ResultModel> ListProvinces(DateTime? from, DateTime? to, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReport(from, to);
                var provinces = await filter.Select(_ => _.Province).Distinct().ToListAsync();
                result.Succeed = true;
                result.Data = provinces;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> ListCBOs(DateTime? from, DateTime? to, CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Reports.BaseFilter().FilterReport(from, to);
                var provinces = await filter.Select(_ => _.Unit.Code).Distinct().ToListAsync();
                result.Succeed = true;
                result.Data = provinces;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }
    }
}
