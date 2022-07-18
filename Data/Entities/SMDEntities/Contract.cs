using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class Contract : BaseEntity
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public bool IsCurrent { get; set; }

        [ForeignKey("CBOId")]
        public virtual Unit CBO { get; set; }
        public Guid CBOId { get; set; }
        [ForeignKey("IPackageId")]
        public virtual ImplementPackage ImplementPackage { get; set; }
        public Guid IPackageId { get; set; }
    }
}
