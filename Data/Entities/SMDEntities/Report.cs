using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.SMDEntities
{
    public class Report : BaseEntity
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

        public Guid IndicatorId { get; set; }
        [ForeignKey("IndicatorId")]
        public virtual Indicator Indicator { get; set; }

        public Guid UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        public virtual ICollection<ReportHistory> ReportHistories { get; set; }

        public Report ShallowCopy()
        {
            return (Report)this.MemberwiseClone();
        }
    }
}
