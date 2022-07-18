using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{

    public class ServiceCreateModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid ServiceFormId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid? InjectionObjectId { get; set; }
    }

    public class ServiceUpdateModel : ServiceCreateModel
    {
        public Guid Id { get; set; }
    }

    public class ServiceModel : ServiceUpdateModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
