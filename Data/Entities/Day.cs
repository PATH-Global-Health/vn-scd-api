using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Constants;

namespace Data.Entities
{
    public class Day : BaseEntity
    {
        public WorkingCalendarStatus Status { get; set; } = 0;
        public DateTime Date { get; set; }
        public Guid CalendarId { get; set; }
        [ForeignKey("CalendarId")]
        public virtual WorkingCalendar WorkingCalendar { get; set; }
    }
}
