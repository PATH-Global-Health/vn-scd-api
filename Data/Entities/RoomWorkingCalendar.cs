using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class RoomWorkingCalendar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid WorkingCalendarId { get; set; }
        [ForeignKey("WorkingCalendarId")]
        public virtual WorkingCalendar WorkingCalendar { get; set; }

        public Guid RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
    }
}
