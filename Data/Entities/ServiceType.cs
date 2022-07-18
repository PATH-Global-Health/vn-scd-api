using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class ServiceType : BaseEntity
    {
        public bool CanChooseDoctor { get; set; }
        public bool CanUseHealthInsurance { get; set; }
        public bool CanChooseHour { get; set; }
        public bool CanPostPay { get; set; }

        public Guid? UnitId { get; set; }

        //public Guid? InjectionObjectId { get; set; }
        //[ForeignKey("InjectionObjectId")]
        //public virtual InjectionObject InjectionObject { get; set; }
    }
}
