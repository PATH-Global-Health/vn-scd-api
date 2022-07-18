using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Entities.SMDEntities;
using Data.Models.SMDModels;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Extenstions
{
    public static class ReportExtensions
    {
        public static IEnumerable<ReportIndividualModel> FilterPositive(this IEnumerable<ReportIndividualModel> models)
        {
            return models.Where(model => model.HIVTestingService.TestResult == "Positive");
        }
        public static IEnumerable<ReportIndividualModel> FilterARVTransport(this IEnumerable<ReportIndividualModel> models)
        {
            return models.FilterPositive().Where(model => model.ReferralService.ServiceName == "ARV");
        }
        public static IEnumerable<ReportIndividualModel> FilterTXNew(this IEnumerable<ReportIndividualModel> models)
        {
            return models.FilterPositive().FilterARVTransport().Where(model => model.VerificationResult.NewCase == "Yes");
        }
        public static IEnumerable<ReportIndividualModel> FilterARVPending(this IEnumerable<ReportIndividualModel> models)
        {
            return models.FilterPositive().FilterARVTransport().Where(model => model.VerificationResult.NewCase == "Pending");
        }
        public static IEnumerable<ReportIndividualModel> FilterNegative(this IEnumerable<ReportIndividualModel> models)
        {
            return models.Where(model => model.HIVTestingService.TestResult == "Negative");
        }
        public static IEnumerable<ReportIndividualModel> FilterPrEPTransport(this IEnumerable<ReportIndividualModel> models)
        {
            return models.FilterNegative().Where(model => model.ReferralService.ServiceName == "PrEP");
        }
        public static IEnumerable<ReportIndividualModel> FilterPrEPNew(this IEnumerable<ReportIndividualModel> models)
        {
            return models.FilterNegative().FilterPrEPTransport().Where(model => model.VerificationResult.NewCase == "Yes");
        }
        public static IEnumerable<ReportIndividualModel> FilterPrEPPending(this IEnumerable<ReportIndividualModel> models)
        {
            return models.FilterNegative().FilterPrEPTransport().Where(model => model.VerificationResult.NewCase == "Pending");
        }

        public static ReportIndicatorTempCollection GetReportIndicatorTempCollection(this IEnumerable<ReportIndividualModel> models)
        {
            var tempCollection = new ReportIndicatorTempCollection();
            var hts_pos = models.FilterPositive();
            tempCollection.HTS_POS = hts_pos;
            var arvTransport = models.FilterARVTransport();
            tempCollection.ARVTransport = arvTransport;
            var tx_new = models.FilterTXNew();
            tempCollection.TX_New = tx_new;
            var arvPending = models.FilterARVPending();
            tempCollection.ARVPending = arvPending;
            var hts_neg = models.FilterNegative();
            tempCollection.HTS_NEG = hts_neg;
            var prepTransport = models.FilterPrEPTransport();
            tempCollection.PrEPTransport = prepTransport;
            var prepNew = models.FilterPrEPNew();
            tempCollection.PrEP_New = prepNew;
            var prepPending = models.FilterPrEPPending();
            tempCollection.PrEPPending = prepPending;
            return tempCollection;
        }

        public static IQueryable<Report> FilterReport(this IQueryable<Report> data, Guid? projectId, Guid? unitId, Guid? indicatorId)
        {
            if (projectId != null)
                data = data.Where(_ => _.Unit.ProjectId == projectId);
            if (unitId != null)
                data = data.Where(_ => _.UnitId == unitId);
            if (indicatorId != null)
                data = data.Where(_ => _.IndicatorId == indicatorId);
            return data;
        }

        public static IQueryable<Report> FilterReport(this IQueryable<Report> data, DateTime? from, DateTime? to)
        {
            if (from != null)
                data = data.Where(_ => _.DateTime >= from);
            if (to != null)
                data = data.Where(_ => _.DateTime <= to);
            return data;
        }

        public static IQueryable<Report> FilterDupeReportForImport(this IQueryable<Report> data,
            IEnumerable<Guid> unitIds = null, IEnumerable<Guid> indicatorIds = null, IEnumerable<DateTime> dates = null)
        {
            data = data.Where(_ => _.Period == ReportPeriod.MONTH);
            if (unitIds != null)
                data = data.Where(_ => unitIds.Contains(_.UnitId));
            if (indicatorIds != null)
                data = data.Where(_ => indicatorIds.Contains(_.IndicatorId));
            if (dates != null)
                data = data.Where(_ => dates.Contains(_.DateTime));
            return data;
        }

        public static Report FindDupe(this IQueryable<Report> data,
            Guid unitId, Guid indicatorId, DateTime date, ReportPeriod period, string province)
        {
            return data.FirstOrDefault(_ => _.UnitId == unitId
                                            && _.IndicatorId == indicatorId
                                            && _.DateTime == date
                                            && _.Period == period
                                            && _.Province == province);
        }

        public static IQueryable<Report> FilterReportModel(this IQueryable<Report> data, ReportGeneralFilterModel model)
        {
            if (model.ReportingPeriod.HasValue)
                data = data.Where(_ => _.Period == model.ReportingPeriod.Value);
            if (model.ImplementingPartners != null && model.ImplementingPartners.Count() > 0)
                data = data.Where(_ => model.ImplementingPartners.Contains(_.Unit.ProjectId));
            if (model.CBOs != null && model.CBOs.Count() > 0)
                data = data.Where(_ => model.CBOs.Contains(_.UnitId));
            if (model.PSNUs != null && model.PSNUs.Count() > 0)
                data = data.Where(_ => model.PSNUs.Contains(_.Province));
            if (model.Indicators != null && model.Indicators.Count() > 0)
                data = data.Where(_ => model.Indicators.Contains(_.IndicatorId));
            if (model.DateTimes != null && model.DateTimes.Count() > 0)
            {
                var datetimes = model.DateTimes.ToReportDateTime();
                data = data.Where(_ => datetimes.Contains(_.DateTime));
            }
            return data;
        }

        public static IQueryable<Report> FilterReportModel(this IQueryable<Report> data, ReportEfficiencyFilterModel model, IEnumerable<Guid> efficiencyIndicators)
        {
            if (model.ReportingPeriod.HasValue)
                data = data.Where(_ => _.Period == model.ReportingPeriod.Value);
            if (model.ImplementingPartners != null && model.ImplementingPartners.Count() > 0)
                data = data.Where(_ => model.ImplementingPartners.Contains(_.Unit.ProjectId));
            if (model.CBOs != null && model.CBOs.Count() > 0)
                data = data.Where(_ => model.CBOs.Contains(_.UnitId));
            if (model.PSNUs != null && model.PSNUs.Count() > 0)
                data = data.Where(_ => model.PSNUs.Contains(_.Unit.Province));
            if (efficiencyIndicators != null && efficiencyIndicators.Count() > 0)
                data = data.Where(_ => efficiencyIndicators.Contains(_.IndicatorId));
            if (model.DateTimes != null && model.DateTimes.Count() > 0)
            {
                var datetimes = model.DateTimes.ToReportDateTime();
                data = data.Where(_ => datetimes.Contains(_.DateTime));
            }
            return data;
        }

        public static IEnumerable<ReportSummaryModel> CalculateAggregateValue(this IEnumerable<IGrouping<dynamic, Report>> data, IEnumerable<Indicator> indicators)
        {
            foreach (var indicator in indicators)
            {
                if (indicator.Type == ReportValueType.INTEGER)
                {
                    var report = data.FirstOrDefault(_ => _.Key.IndicatorId == indicator.Id);
                    if (report != null)
                        yield return new ReportSummaryModel { IndicatorId = report.Key.IndicatorId, Value = report.Sum(x => x.Value), ValueType = ReportValueType.INTEGER };
                    else
                        yield return new ReportSummaryModel { IndicatorId = indicator.Id, Value = null, ValueType = ReportValueType.INTEGER };
                }
            }
            //foreach (var indicatorGroup in data)
            //{
            //    if (indicatorGroup.Key.ValueType == ReportValueType.INTEGER)
            //    {
            //        yield return new ReportSummaryModel { IndicatorId = indicatorGroup.Key.IndicatorId, Value = indicatorGroup.Sum(x => x.Value), ValueType = ReportValueType.INTEGER };
            //    }
            //}
        }

        public static IEnumerable<ReportSummaryModel> CalculateAggregatePercentageValue(this IEnumerable<ReportSummaryModel> data, IEnumerable<Indicator> indicators)
        {
            foreach (var indicator in indicators)
            {
                if (indicator.Type == ReportValueType.PERCENTAGE)
                {
                    var numerator = data.FirstOrDefault(_ => _.IndicatorId == indicator.NumeratorIndicatorId);
                    var denominator = data.FirstOrDefault(_ => _.IndicatorId == indicator.DenominatorIndicatorId);
                    yield return new ReportSummaryModel { IndicatorId = indicator.Id, Value = numerator == null || denominator == null ? null : UtilityMethods.Percentage(numerator.Value, denominator.Value), ValueType = ReportValueType.PERCENTAGE };
                }
            }
        }

        public static IEnumerable<ReportSummaryModel> CalculateAggregatePaymentValue(this IEnumerable<IGrouping<dynamic, Report>> data, Indicator paymentIndicator)
        {
            if (paymentIndicator.Type == ReportValueType.MONEY)
            {
                var paymentReports = data.FirstOrDefault(_ => _.Key.IndicatorId == paymentIndicator.Id);
                if (paymentReports != null)
                {
                    var groupPayments = paymentReports.GroupBy(_ => _.PackageCode);
                    for (int i = 1; i <= 5; i++)
                    {

                        var key = $"P0{i % 5}";
                        var report = groupPayments.FirstOrDefault(_ => _.Key == key);
                        if (report != null)
                        {
                            yield return new ReportSummaryModel
                            {
                                IndicatorId = paymentIndicator.Id,
                                PackageCode = report != null ? report.Key : key,
                                PackageNumber = report?.Count(),
                                Value = report?.Sum(_ => _.Value),
                                ValueType = ReportValueType.MONEY
                            };
                        }
                        else
                        {
                            yield return new ReportSummaryModel
                            {
                                IndicatorId = paymentIndicator.Id,
                                PackageCode = key,
                                PackageNumber = null,
                                Value = null,
                                ValueType = ReportValueType.MONEY
                            };
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        var key = $"P0{i % 5}";
                        yield return new ReportSummaryModel
                        {
                            IndicatorId = paymentIndicator.Id,
                            PackageCode = key,
                            PackageNumber = null,
                            Value = null,
                            ValueType = ReportValueType.MONEY
                        };
                    }
                }
            }
        }

        public static IEnumerable<ReportBartChartModel> CalculateBarChartValue(this IEnumerable<IGrouping<dynamic, Report>> data, IEnumerable<Guid> indicatorIds)
        {
            var initGroup = data.GroupBy(_ => _.Key.GroupByType);
            foreach (var group in initGroup)
            {
                foreach (var indicatorId in indicatorIds)
                {
                    var report = group.FirstOrDefault(_ => _.Key.IndicatorId == indicatorId);
                    if (report != null)
                    {
                        if (report.Key.ValueType == ReportValueType.INTEGER)
                            yield return new ReportBartChartModel { IndicatorId = report.Key.IndicatorId, Label = report.Key.GroupByType, Data = report.Sum(x => x.Value), Target = report.Sum(x => x.TargetValue) };
                        if (report.Key.ValueType == ReportValueType.MONEY)
                        {
                            foreach (var item in report.ToList().GroupBy(s => s.PackageCode))
                            {
                                yield return new ReportBartChartModel { 
                                    IndicatorId = item.FirstOrDefault().IndicatorId, 
                                    Label = report.Key.GroupByType, 
                                    Data = item.Sum(x => x.Value), PackageNumber = item.Count(), PackageCode = item.Key };
                            };
                        }
                    }
                    else
                    {
                        yield return new ReportBartChartModel { IndicatorId = indicatorId, Label = group.Key, Data = null };
                    }
                }
            }
            //foreach (var indicatorGroup in data)
            //{
            //    if (indicatorGroup.Key)
            //    if (indicatorGroup.Key.ValueType == ReportValueType.INTEGER)
            //        yield return new ReportBartChartModel { IndicatorId = indicatorGroup.Key.IndicatorId, Label = indicatorGroup.Key.GroupByType, Data = indicatorGroup.Sum(x => x.Value) };
            //    if (indicatorGroup.Key.ValueType == ReportValueType.MONEY)
            //        yield return new ReportBartChartModel { IndicatorId = indicatorGroup.Key.IndicatorId, Label = indicatorGroup.Key.GroupByType, Data = indicatorGroup.Sum(x => x.Value), PackageNumber = indicatorGroup.Count() };
            //}
        }

        public static IEnumerable<IGrouping<ReportIndividualGroupedModel, ReportIndividualModel>> GroupByForIndividualImport(this IEnumerable<ReportIndividualModel> filter)
        {
            return filter.GroupBy(_ => new ReportIndividualGroupedModel { CBOName = _.CBOName, ReportingPeriod = _.ReportingPeriod, PSNU = _.PSNU });
        }

        public static EntityEntry<Report> UpdateReport(this AppDbContext context, Report oldEntity, Report newEntity, string username)
        {
            // create new history
            ReportHistory history = oldEntity.Adapt<ReportHistory>();
            history.ReportId = oldEntity.Id;
            context.ReportHistories.Add(history);
            // update report
            newEntity.CreateBy = username;
            newEntity.DateUpdated = DateTime.Now;
            newEntity.Adapt(oldEntity);
            return context.Reports.Update(oldEntity);
        }

        public static object ReCalculateReport(this AppDbContext context, Guid CBOId, DateTime reportingPeriod, string username)
        {
            return null;
        }

        #region For patientInfo
        public static async Task<IEnumerable<Report>> ToReportAsync(this IDictionary<Indicator, IEnumerable<PatientInfo>> tempCollection, IEnumerable<Unit> cbos, Indicator indicator)
        {
            return await Task.Run(() =>
            {
                bool t = tempCollection.TryGetValue(indicator, out var resultModels);

                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var groupByPeriod = resultModels.GroupByForIndividualImport();
                var result = groupByPeriod.Select(_ =>
                {
                    var cboKey = _.Key.CBOName.ToUpper();
                    var cbo = cbos.FirstOrDefault(cbo => cbo.Name.ToUpper().Equals(cboKey) || cbo.Code.ToUpper().Equals(cboKey));
                    if (cbo == null)
                        throw new Exception($"CBO with name {_.Key.CBOName} not found.");
                    return new Report
                    {
                        IndicatorId = indicator.Id,
                        Value = _.Count(),
                        ValueType = ReportValueType.INTEGER,
                        Period = Data.Constants.ReportPeriod.MONTH,
                        DateTime = _.Key.ReportingPeriod,
                        UnitId = cbo.Id,
                        CreatedMethod = CreatedMethod.IMPORT,
                        Province = _.Key.PSNU
                    };
                });
                return result;
            });
        }

        private static IEnumerable<IGrouping<dynamic, PatientInfo>> GroupByForIndividualImport(this IEnumerable<PatientInfo> filter)
        {
            return filter.GroupBy(_ => new { CBOName = _.CBOName, ReportingPeriod = _.ReportingPeriod, PSNU = _.PSNU });
        }

        public static async Task<IEnumerable<Report>> ToReportForPatientInfoAsync(this IDictionary<Indicator, IEnumerable<PatientInfo>> tempCollection, IEnumerable<Unit> cbos, Indicator indicator)
        {
            return await Task.Run(() =>
            {
                bool t = tempCollection.TryGetValue(indicator, out var resultModels);

                if (resultModels == null || resultModels.Count() <= 0)
                    return null;
                var groupByPeriod = resultModels.GroupByForPatientInfo();
                var result = groupByPeriod.Select(_ => new Report
                {
                    IndicatorId = indicator.Id,
                    Value = _.Count(),
                    ValueType = ReportValueType.INTEGER,
                    Period = Data.Constants.ReportPeriod.MONTH,
                    DateTime = _.Key.ReportingPeriod,
                    UnitId = cbos.FirstOrDefault(cbo => cbo.Code == _.Key.CBOCode).Id,
                    CreatedMethod = CreatedMethod.IMPORT,
                    Province = _.Key.PSNU,
                    TargetValue = _.Count(),
                });
                return result;
            });
        }

        private static IEnumerable<IGrouping<dynamic, PatientInfo>> GroupByForPatientInfo(this IEnumerable<PatientInfo> filter)
        {
            return filter.GroupBy(_ => new { CBOCode = _.CBOCode, ReportingPeriod = _.ReportingPeriod, PSNU = _.PSNU });
        }

        public static IDictionary<Indicator, IEnumerable<PatientInfo>> GetReportIndicatorTempCollection(this IEnumerable<PatientInfo> models, IEnumerable<Indicator> indicators)
        {
            var tempCollection = new Dictionary<Indicator, IEnumerable<PatientInfo>>();
            foreach (var item in indicators)
            {
                switch (item.IndicatorType)
                {
                    case IndicatorType.POSITIVE:
                        tempCollection.Add(item, models.FilterPositive());
                        break;
                    case IndicatorType.NEGATIVE:
                        tempCollection.Add(item, models.FilterNegative());
                        break;
                    case IndicatorType.ARV_TRANSPORT:
                        tempCollection.Add(item, models.FilterARVTransport());
                        break;
                    case IndicatorType.ARV_PENDING:
                        tempCollection.Add(item, models.FilterARVPending());
                        break;
                    case IndicatorType.TX_NEW:
                        tempCollection.Add(item, models.FilterTXNew());
                        break;
                    case IndicatorType.PREP_TRANSPORT:
                        tempCollection.Add(item, models.FilterPrEPTransport());
                        break;
                    case IndicatorType.PREP_PENDING:
                        tempCollection.Add(item, models.FilterPrEPPending());
                        break;
                    case IndicatorType.PREP_NEW:
                        tempCollection.Add(item, models.FilterPrEPNew());
                        break;
                    default:
                        break;
                }
            }
            return tempCollection;
        }

        public static IEnumerable<PatientInfo> FilterPositive(this IEnumerable<PatientInfo> models)
        {
            return models.Where(model => model.TestResult == "POSITIVE");
        }
        public static IEnumerable<PatientInfo> FilterARVTransport(this IEnumerable<PatientInfo> models)
        {
            return models.FilterPositive().Where(model => model.ServiceName == "ARV");
        }
        public static IEnumerable<PatientInfo> FilterTXNew(this IEnumerable<PatientInfo> models)
        {
            return models.FilterPositive().FilterARVTransport().Where(model => model.NewCase == "Yes");
        }
        public static IEnumerable<PatientInfo> FilterARVPending(this IEnumerable<PatientInfo> models)
        {
            return models.FilterPositive().FilterARVTransport().Where(model => model.NewCase == "Pending");
        }
        public static IEnumerable<PatientInfo> FilterNegative(this IEnumerable<PatientInfo> models)
        {
            return models.Where(model => model.TestResult == "NEGATIVE");
        }
        public static IEnumerable<PatientInfo> FilterPrEPTransport(this IEnumerable<PatientInfo> models)
        {
            return models.FilterNegative().Where(model => model.ServiceName == "PrEP");
        }
        public static IEnumerable<PatientInfo> FilterPrEPNew(this IEnumerable<PatientInfo> models)
        {
            return models.FilterNegative().FilterPrEPTransport().Where(model => model.NewCase == "Yes");
        }
        public static IEnumerable<PatientInfo> FilterPrEPPending(this IEnumerable<PatientInfo> models)
        {
            return models.FilterNegative().FilterPrEPTransport().Where(model => model.NewCase == "Pending");
        }
        #endregion

        public static void UpdateReport(this AppDbContext context, string username, ICollection<Report> reports, ICollection<Indicator> efficiencyIndicators, IEnumerable<DateTime> dates, IEnumerable<Target> targets = null, IEnumerable<ImplementPackage> ipackages = null)
        {
            var efficiencyIndicatorIds = efficiencyIndicators.Select(i => i.Id);
            var cbos = reports.Select(_ => _.UnitId);
            var indicators = reports.Select(_ => _.IndicatorId);
            //var dates = reports.Select(_ => _.DateTime);

            var oldReports = context.Reports.BaseFilter().FilterDupeReportForImport(cbos, indicators, dates).ToList();
            if (oldReports != null)
            {
                foreach (var oldReport in oldReports)
                {
                    var newReport = reports.FirstOrDefault(_ => _.UnitId == oldReport.UnitId
                                                                && _.IndicatorId == oldReport.IndicatorId
                                                                && _.DateTime == oldReport.DateTime);

                    if (newReport != null)
                    {
                        if (efficiencyIndicatorIds.Contains(oldReport.IndicatorId) && oldReport.TargetValue == null)
                        {
                            if (newReport.ValueType == ReportValueType.MONEY)
                            {
                                newReport.AddTargetAmountToReport(ipackages);
                            }
                        }
                        context.UpdateReport(oldReport, newReport, username);
                        reports.Remove(newReport);
                    }
                    else
                    {
                        var vm = oldReport.Adapt<ReportViewModel>();
                        newReport = vm.Adapt<Report>();
                        newReport.Value = 0;
                        context.UpdateReport(oldReport, newReport, username);
                    }
                }
            }
        }

        public static void UpdateReportWithDeletedPatientInfo(this AppDbContext context, string username, IEnumerable<PatientInfo> deletedPatientInfos, IEnumerable<Indicator> integerIndicators, IEnumerable<DateTime> dates, IEnumerable<Target> targets = null, IEnumerable<ImplementPackage> ipackages = null)
        {
            var iIndicatorIds = integerIndicators.Select(i => i.Id);
            var cbos = deletedPatientInfos.Select(_ => _.CBOId);
            //var dates = reports.Select(_ => _.DateTime);

            var oldReports = context.Reports.BaseFilter().FilterDupeReportForImport(cbos, iIndicatorIds, dates).ToList();
            if (oldReports != null)
            {
                foreach (var oldReport in oldReports)
                {
                    var vm = oldReport.Adapt<ReportViewModel>();
                    var newReport = vm.Adapt<Report>();
                    newReport.Value = 0;
                    context.UpdateReport(oldReport, newReport, username);
                }
            }
        }

        //private static ICollection<Target> GetTargetsByUnit(Guid unitId)
        //{
        //    return _dbContext.Targets.Where(_ => _.ImplementPackage.Contracts.Any(c => c.CBOId == unitId && c.IsCurrent)).ToList();
        //}

        private static ICollection<Target> GetTargetsByUnit(IEnumerable<Target> targets, Guid unitId, DateTime date)
        {
            return targets.Where(_ => _.ImplementPackage.Contracts.Any(
                                                                    c => c.CBOId == unitId
                                                                    && c.Start <= date
                                                                    && (!c.End.HasValue || c.End >= date)
                                                                    ))
                            .ToList();
        }

        public static void AddTargetToReportDeprecated(this Report entity, IEnumerable<Target> targets)
        {
            if (targets != null && entity != null && entity.ValueType == ReportValueType.INTEGER)
            {
                var unitTargets = GetTargetsByUnit(targets, entity.UnitId, entity.DateTime);
                var target = unitTargets.FirstOrDefault(_ => _.IndicatorId == entity.IndicatorId);
                if (target != null)
                    entity.TargetValue = target.Quantity;
            }
        }

        private static ImplementPackage GetIPackageByUnit(IEnumerable<ImplementPackage> data, string province, string packageCode)
        {
            return data.FirstOrDefault(_ => _.Province == province
                                         && _.Package.Code == packageCode);
        }

        private static ImplementPackage GetIPackageByUnit(IEnumerable<ImplementPackage> data, Guid unitId, string packageCode, DateTime date)
        {
            return data.FirstOrDefault(_ => _.Contracts.Any(c => c.CBOId == unitId
                                                                    && c.Start <= date
                                                                    && (!c.End.HasValue || c.End >= date))
                                            && _.Package.Code == packageCode
                                            );
        }

        public static void AddTargetAmountToReport(this Report entity, IEnumerable<ImplementPackage> data)
        {
            if (data != null && entity != null && entity.ValueType == ReportValueType.MONEY)
            {
                var ipackage = GetIPackageByUnit(data, entity.Province, entity.PackageCode);
                if (ipackage != null)
                    entity.TargetValue = ipackage.TotalAmount;
            }
        }

        public static async Task<object> RecalIndicatorEffeciency(this AppDbContext _dbContext, List<RecalByPaymentModel> cases)
        {
            cases.ForEach(c => {
                //get payment report
                var payment_report = _dbContext.Reports.FirstOrDefault(f => f.DateTime == c.DateTime
                                                                        && f.UnitId == c.UnitId
                                                                        && f.Province == c.Province
                                                                        && f.ValueType == ReportValueType.MONEY);
                if (payment_report == null)
                {
                    return;
                }
                var implemented_package = _dbContext.ImplementPackages.FirstOrDefault(f => f.Package.Code == payment_report.PackageCode
                                                                                        && f.Province == payment_report.Province);
                if (implemented_package == null)
                {
                    return;
                }
                if (payment_report.TargetValue == null)
                {
                    payment_report.TargetValue = implemented_package.TotalAmount;
                }
                var reports = _dbContext.Reports.Where(w => w.UnitId == c.UnitId
                                                        && w.Province == c.Province
                                                        && w.DateTime == c.DateTime
                                                        && implemented_package.Targets
                                                                            .Select(_ => _.IndicatorId).Contains(w.IndicatorId));
                reports.ForEach(report =>
                {
                    var target = implemented_package.Targets.FirstOrDefault(_ => _.IndicatorId == report.IndicatorId);
                    report.TargetValue = target.Quantity;
                    report.PackageCode = payment_report.PackageCode;
                    _dbContext.Reports.Update(report);
                });
            });
            return await _dbContext.SaveChangesAsync();
        }
    }
}
