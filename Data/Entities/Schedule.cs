using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Schedule : BaseEntity
    {
        //public WorkingCalendarStatus Status { get; set; } = 0;
        public string From { get; set; }
        public string To { get; set; }
        public Guid DayId { get; set; }
        [ForeignKey("DayId")]
        public virtual Day Day { get; set; }
    }
}
