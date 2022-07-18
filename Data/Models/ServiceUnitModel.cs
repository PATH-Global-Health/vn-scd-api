using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ServiceUnitCreateModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public float Price { get; set; }
        [Required]
        public Guid ServiceId { get; set; }
        [Required]
        public Guid UnitId { get; set; }
    }

    public class ServiceUnitUpdateModel : ServiceUnitCreateModel
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class ServiceUnitModel : ServiceUnitUpdateModel
    {
    }
}
