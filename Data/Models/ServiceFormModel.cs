using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ServiceFormModel : ServiceFormUpdateModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
    public class ServiceFormCreateModel
    {
        [Required]
        public string Name { get; set; }
    }
    public class ServiceFormUpdateModel : ServiceFormCreateModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
