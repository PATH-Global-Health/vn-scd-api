using Data.Constants;
using Data.Models.CustomModels;
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.SMDModels
{
    public class ReportCreateModel
    {
        [Required]
        public ReportPeriod Period { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public Guid IndicatorId { get; set; }
        [Required]
        public Guid UnitId { get; set; }
        [Required]
        public string Province { get; set; }
        public string PackageCode { get; set; }
    }

    public class ReportUpdateModel : ReportCreateModel
    {
        public Guid Id { get; set; }
    }

    public class ReportViewModel : ReportUpdateModel {
        public string CBOName { get; set; }
        public double? TargetValue { get; set; }
    }

    public interface IValidator
    {
        IEnumerable<ValidationResult> Validate(ValidationContext context);
    }

    public class ReportIndividualModel : ReportBaseModel
    {
        public ReportIndividualModel()
        {
            HIVTestingService = new HIVTestingServiceModel();
            ReferralService = new ReferralServiceModel();
            VerificationResult = new VerificationResultModel();
        }
        public string SupporterName { get; set; }
        public string ReachCode { get; set; }
        //[DateInReportingPeriod(nameof(ReportingPeriod))]
        public HIVTestingServiceModel HIVTestingService { get; set; }
        //[DateInReportingPeriod(nameof(ReportingPeriod))]
        public ReferralServiceModel ReferralService { get; set; }
        [StringRange(AllowableValues = new[] { "HTC Site", "ARV Site", "HTC & ARV Site", "PrEP Site" }, AllowNullOrEmpty = true)]
        public string ReferralSlip { get; set; }
        //[DateInReportingPeriod(nameof(ReportingPeriod))]
        public VerificationResultModel VerificationResult { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class ReportPaymentModel : ReportBaseModel
    {
        public string PackageCode { get; set; }
        public double TotalAmount { get; set; }
        public DateTime DateOfPayment { get; set; }
    }

    public class ReportBaseModel
    {
        public int Row { get; set; }
        public int No { get; set; }
        public string PSNU { get; set; }
        public string MoPName { get; set; }
        [Required]
        public string CBOName { get; set; }
        [Required]
        public DateTime ReportingPeriod { get; set; }
        public string Note { get; set; }
    }

    public class HIVTestingServiceModel : IndicatorReportModel
    {
        public string LayTestingCode { get; set; }
        public string HTCTestCode { get; set; }
        public string HTCSite { get; set; }
        [StringRange(AllowableValues = new[] { "Positive", "Negative" }, IgnoreCase = true, AllowNullOrEmpty = true)]
        public string TestResult { get; set; }
        //public DateTime TestingDate { get; set; }
    }

    public class ReferralServiceModel : IndicatorReportModel
    {
        [StringRange(AllowableValues = new[] { "ARV", "PrEP" }, AllowNullOrEmpty = true)]
        public string ServiceName { get; set; }
        public string ClientID { get; set; }
        public string FacilityName { get; set; }
        //public DateTime ReferralDate { get; set; }
    }

    public class VerificationResultModel : IndicatorReportModel
    {
        [StringRange(AllowableValues = new[] { "Yes", "No", "Pending" }, AllowNullOrEmpty = true)]
        public string NewCase { get; set; }
        //public DateTime VerificationDate { get; set; }
    }

    public class IndicatorReportModel
    {
        public DateTime? ReportDate { get; set; }
    }

    public class ReportAggregateModel
    {
        public int Row { get; set; }
        [Required]
        public ReportPeriod Period { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public string CBOCode { get; set; }
        [Required]
        public string PSNU { get; set; }
        [Required]
        public string IndicatorCode { get; set; }
        [Required]
        public double Value { get; set; }
    }

    public class ReportIndicatorTempCollection
    {
        public IEnumerable<ReportIndividualModel> HTS_POS { get; set; }
        public IEnumerable<ReportIndividualModel> ARVTransport { get; set; }
        public IEnumerable<ReportIndividualModel> TX_New { get; set; }
        public IEnumerable<ReportIndividualModel> ARVPending { get; set; }
        public IEnumerable<ReportIndividualModel> HTS_NEG { get; set; }
        public IEnumerable<ReportIndividualModel> PrEPTransport { get; set; }
        public IEnumerable<ReportIndividualModel> PrEP_New { get; set; }
        public IEnumerable<ReportIndividualModel> PrEPPending { get; set; }
    }

    public class ReportFilterModel
    {
        public ReportPeriod? ReportingPeriod { get; set; }
        public IEnumerable<Guid?> ImplementingPartners { get; set; }
        public IEnumerable<string> PSNUs { get; set; }
        public IEnumerable<Guid> CBOs { get; set; }
        public IEnumerable<DateTime> DateTimes { get; set; }
    }

    public class ReportGeneralFilterModel : ReportFilterModel
    {
        public IEnumerable<Guid> Indicators { get; set; }
    }

    public class ReportBarChartFilterModel : ReportGeneralFilterModel, IFilterModel
    {
        public ReportGroupByType GroupByType { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = int.MaxValue;
    }

    public class ReportEfficiencyFilterModel : ReportFilterModel { }

    public class ReportGroupByModel
    {
        public Guid IndicatorId { get; set; }
        public ReportValueType ValueType { get; set; }
    }

    public class ReportSummaryModel
    {
        public Guid IndicatorId { get; set; }
        public double? Value { get; set; }
        public ReportValueType ValueType { get; set; }
        public string PackageCode { get; set; }
        public int? PackageNumber { get; set; }
    }

    public class ReportBartChartModel
    {
        public Guid IndicatorId { get; set; }
        public string Label { get; set; }
        public string PackageCode { get; set; }
        public double? Data { get; set; }
        public double? Target { get; set; }
        public int? PackageNumber { get; set; }
    }

    public class ReportEfficiencyModel : ReportViewModel
    {
        public string ProjectName { get; set; }
    }

    public class ReportCreateModelV2
    {
        [Required]
        public ReportPeriod Period { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public Guid UnitId { get; set; }

        public IEnumerable<ReportCreateShort> IndicatorValues { get; set; }
    }

    public class ReportCreateShort
    {
        public Guid IndicatorId { get; set; }
        public double Value { get; set; }
    }

    public class ReportIndividualGroupedModel
    {
        public string CBOName { get; set; }
        public DateTime ReportingPeriod { get; set; }
        public string PSNU { get; set; }
    }

    public class RecalByPaymentModel
    {
        public string Province { get; set; }
        public DateTime DateTime { get; set; }
        public Guid UnitId { get; set; }
    }

    public class ReportExportExcelModel
    {
        [Column(1, "Năm - Year")]
        public int Year { get; set; }

        [Column(2, "Tháng - Month")]
        public int Month { get; set; }

        [Column(3, "Dự án - IPs")]
        public string Project { get; set; }

        [Column(4, "Tỉnh/Thành - Province")]
        public string Province { get; set; }

        [Column(5, "CBO")]
        public string CBOName { get; set; }

        [Column(6, "Chỉ số - Indicator")]
        public string IndicatorName { get; set; }
        
        [Column(7, "Gói - Package")]
        public string PackageCode { get; set; }
        
        [Column(8, "Giá trị - Value")]
        public double Value { get; set; }

        public void DoAfterMapping()
        {

        }
    }
}
