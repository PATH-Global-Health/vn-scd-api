using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Service : Job
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool CommonResource { get; set; }
        public double? Price { get; set; }

        //public Guid HospitalId { get; set; }
        //[ForeignKey("HospitalId")]
        //public virtual Hospital Hospital { get; set; }

        public Guid ServiceFormId { get; set; }
        [ForeignKey("ServiceFormId")]
        public virtual ServiceForm ServiceForm { get; set; }

        public Guid ServiceTypeId { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }

        public Guid? InjectionObjectId { get; set; }
        [ForeignKey("InjectionObjectId")]
        public virtual InjectionObject InjectionObject { get; set; }
    }
}
