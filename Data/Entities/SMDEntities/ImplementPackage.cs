using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class ImplementPackage : BaseEntity
    {
        public string Province { get; set; }
        public double TotalAmount { get; set; }

        [ForeignKey("PackageId")]
        public virtual Package Package { get; set; }
        public Guid PackageId { get; set; }
        public virtual ICollection<Target> Targets { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
