using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class KPI : BaseEntity
    {
        public double From { get; set; }
        public double To { get; set; }
        public string Color { get; set; }

        public Guid IndicatorId { get; set; }
        [ForeignKey("IndicatorId")]
        public virtual Indicator Indicator { get; set; }

        public void FormatColor()
        {
            Color = Color.Trim().ToUpper();
        }

        public void Validate()
        {
            if (From > To)
                throw new Exception("'From' cannot be greater than 'To'");
        }
    }
}
