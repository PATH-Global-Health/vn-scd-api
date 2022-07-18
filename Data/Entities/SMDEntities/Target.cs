using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class Target : BaseEntity
    {
        public int Quantity { get; set; }

        [ForeignKey("IPackageId")]
        public virtual ImplementPackage ImplementPackage { get; set; }
        public Guid IPackageId { get; set; }
        [ForeignKey("IndicatorId")]
        public virtual Indicator Indicator { get; set; }
        public Guid IndicatorId { get; set; }
    }
}
