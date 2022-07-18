using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class UnitImage : BaseEntity
    {
        public byte[] Image { get; set; }

        public Guid UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }
    }
}
