using Data.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models.SMDModels
{
    public class PatientInfoCreateModel
    {
        [Required]
        public string PSNU { get; set; }
        public string MoPName { get; set; }
        public string CBOName { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 1)]
        public string CBOCode { get; set; }
        public string SupporterName { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 1)]
        public string ReachCode { get; set; }
        public string LayTestingCode { get; set; }
        public string HTCTestCode { get; set; }
        public string HTCSite { get; set; }
        [StringRange(AllowableValues = new[] { "Positive", "Negative" }, IgnoreCase = true, AllowNullOrEmpty = true)]
        public string TestResult { get; set; }
        public DateTime? DateOfTesting { get; set; }
        [StringRange(AllowableValues = new[] { "ARV", "PrEP"})]
        public string ServiceName { get; set; }
        public string ClientID { get; set; }
        public string FacilityName { get; set; }
        public DateTime? DateOfReferral { get; set; }
        [StringRange(AllowableValues = new[] { "HTC Site", "ARV Site", "HTC & ARV Site", "PrEP Site" }, AllowNullOrEmpty = true)]
        public string ReferralSlip { get; set; }
        [StringRange(AllowableValues = new[] { "Yes", "No", "Pending" }, AllowNullOrEmpty = true)]
        public string NewCase { get; set; }
        public DateTime? DateOfVerification { get; set; }
        public DateTime ReportingPeriod { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Note { get; set; }

        public void FormatData()
        {
            ReportingPeriod = ReportingPeriod.ToReportDatetime();
        }
    }

    public class PatientInfoUpdateModel : PatientInfoCreateModel
    {
        public Guid Id { get; set; }
    }

    public class PatientInfoViewModel {
        public Guid Id { get; set; }
        public string PSNU { get; set; }
        public string MoPName { get; set; }
        public string CBOName { get; set; }
        public string CBOCode { get; set; }
        public string SupporterName { get; set; }
        public string ReachCode { get; set; }
        public string LayTestingCode { get; set; }
        public string HTCTestCode { get; set; }
        public string HTCSite { get; set; }
        public string TestResult { get; set; }
        public DateTime? DateOfTesting { get; set; }
        public string ServiceName { get; set; }
        public string ClientID { get; set; }
        public string FacilityName { get; set; }
        public DateTime? DateOfReferral { get; set; }
        public string ReferralSlip { get; set; }
        public string NewCase { get; set; }
        public DateTime? DateOfVerification { get; set; }
        public DateTime ReportingPeriod { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Note { get; set; }
    }

    public class PatientInfoFilterModel : IFilterModel
    {
        public IEnumerable<DateTime> DateTimes { get; set; }
        public IEnumerable<string> PSNUs { get; set; }
        public IEnumerable<string> ReferralServices { get; set; }
        public string TestResult { get; set; }
        public DateTime? DateUpdatedFrom { get; set; }
        public DateTime? DateUpdatedTo { get; set; }
        public int PageIndex { get; set; }
        public int PageSize {get; set; } = int.MaxValue;
    }

    public class PatientInfoHistoryModel : PatientInfoViewModel
    {
        public Guid PatientInfoId { get; set; }
    }
}
