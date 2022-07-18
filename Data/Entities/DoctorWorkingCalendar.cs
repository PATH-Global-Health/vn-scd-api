using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class DoctorWorkingCalendar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid WorkingCalendarId { get; set; }
        [ForeignKey("WorkingCalendarId")]
        public virtual WorkingCalendar WorkingCalendar { get; set; }

        public Guid DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}
