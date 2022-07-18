using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class PatientInfo : BaseEntity
    {
        public string PSNU { get; set; }
        public string MoPName { get; set; }
        public string CBOName { get; set; }
        [StringLength(128, MinimumLength = 1)]
        public string CBOCode { get; set; }
        public Guid CBOId { get; set; }
        public string SupporterName { get; set; }
        [StringLength(128, MinimumLength = 1)]
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
        public DateTime UpdatedDate { get; set; }
        public string Note { get; set; }
        public string CreateBy { get; set; }
        public CreatedMethod CreatedMethod { get; set; }

        public virtual ICollection<PatientInfoHistory> PatientInfoHistories { get; set; }

        public void FormatResult()
        {
            if (TestResult != null)
                TestResult = TestResult.Trim().ToUpper();
            ReportingPeriod = new DateTime(ReportingPeriod.Year, ReportingPeriod.Month, DateTime.DaysInMonth(ReportingPeriod.Year, ReportingPeriod.Month));
        }
    }
}
