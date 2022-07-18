using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ServiceTypeCreateModel
    {
        [Required]
        public bool CanChooseDoctor { get; set; }
        [Required]
        public bool CanUseHealthInsurance { get; set; }
        [Required]
        public bool CanChooseHour { get; set; }
        [Required]
        public bool CanPostPay { get; set; }
        public string Description { get; set; }
        public Guid? UnitId { get; set; }
        public Guid? InjectionObjectId { get; set; }
    }

    public class ServiceTypeUpdateModel : ServiceTypeCreateModel
    {
        public Guid Id { get; set; }
    }

    public class ServiceTypeModel : ServiceTypeUpdateModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
