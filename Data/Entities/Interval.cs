using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Interval : BaseEntity
    {
        public WorkingCalendarStatus Status { get; set; } = 0;
        public string From { get; set; }
        public string To { get; set; }
        public bool IsAvailable { get; set; }
        public Guid ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NumId { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
