using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class RoomCreateModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid UnitId { get; set; }
    }

    public class RoomUpdateModel : RoomCreateModel
    {
        public Guid Id { get; set; }
    }

    public class RoomModel : RoomUpdateModel
    {
        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public UnitModel Unit { get; set; }
    }
}
