using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class ReportHistory : BaseEntity
    {
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
        [ForeignKey("ReportId")]
        public virtual Report Report { get; set; }
    }
}
