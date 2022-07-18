using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class ServiceUnit : BaseEntity
    {
        public string Code { get; set; }

        public float Price { get; set; }

        public Guid ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }

        public Guid UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }
    }
}
