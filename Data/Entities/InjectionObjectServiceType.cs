using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class InjectionObjectServiceType
    {
        [Key]
        public Guid InjectionObjectId { get; set; }
        [ForeignKey("InjectionObjectId")]
        public virtual InjectionObject InjectionObject { get; set; }

        [Key]
        public Guid ServiceTypeId { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
    }
}
