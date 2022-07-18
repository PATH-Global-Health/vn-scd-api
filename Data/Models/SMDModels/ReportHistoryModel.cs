using Data.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.SMDModels
{
    public class ReportHistoryViewModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public ReportPeriod Period { get; set; }
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
        public ReportValueType ValueType { get; set; }
        public CreatedMethod CreatedMethod { get; set; }
        public string PackageCode { get; set; }
        public double? TargetValue { get; set; }
        public string Province { get; set; }

        //
        public string CreateBy { get; set; }
        public Guid ReportId { get; set; }
    }
}
