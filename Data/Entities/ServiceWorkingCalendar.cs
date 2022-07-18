using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class ServiceWorkingCalendar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid WorkingCalendarId { get; set; }
        [ForeignKey("WorkingCalendarId")]
        public virtual WorkingCalendar WorkingCalendar { get; set; }

        public Guid ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}
