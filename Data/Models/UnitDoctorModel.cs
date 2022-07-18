using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class UnitDoctorCreateModel
    {
        [Required]
        [MaxLength(16)]
        public string Code { get; set; }
        [Required]
        public Guid UnitId { get; set; }
        [Required]
        public Guid DoctorId { get; set; }
    }

    public class UnitDoctorUpdateModel : UnitDoctorCreateModel
    {
        public Guid Id { get; set; }
    }

    public class UnitDoctorModel : UnitDoctorUpdateModel
    {

    }
}
