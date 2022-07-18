using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class SchedulePlace
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid PlaceId { get; set; }
        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }

        public Guid ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }
    }
}
