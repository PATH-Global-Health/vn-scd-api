using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class Indicator : BaseCodeEntity
    {
        public string Name { get; set; }
        public ReportValueType Type { get; set; }
        public Guid? NumeratorIndicatorId { get; set; }
        public Guid? DenominatorIndicatorId { get; set; }
        public IndicatorType IndicatorType { get; set; }

        public bool Validate()
        {
            if (Type == ReportValueType.PERCENTAGE && (!NumeratorIndicatorId.HasValue || !DenominatorIndicatorId.HasValue))
                return false;
            return true;
        }

        [ForeignKey("NumeratorIndicatorId")]
        public virtual Indicator NumeratorIndicator { get; set; }
        [ForeignKey("DenominatorIndicatorId")]
        public virtual Indicator DenominatorIndicator { get; set; }
        public virtual ICollection<KPI> KPIs { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
