using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class UnitTypeCreateModel
    {
        [Required]
        [MaxLength(16)]
        public string Code { get; set; }
        [Required]
        [MaxLength(16)]
        public string TypeName { get; set; }
        [MaxLength(1024)]
        public string Description { get; set; }
    }

    public class UnitTypeUpdateModel : UnitTypeCreateModel
    {
        public Guid Id { get; set; }
    }
    public class UnitTypeModel : UnitTypeUpdateModel
    {

    }
}
