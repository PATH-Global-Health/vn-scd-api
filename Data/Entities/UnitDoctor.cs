using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class UnitDoctor : BaseEntity
    {
        public string Code { get; set; }

        public string Username { get; set; }

       

        public Guid? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }

        public Guid DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
