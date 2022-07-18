using Data.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class WorkingCalendar : BaseEntity
    {
        public WorkingCalendarStatus Status { get; set; } = 0;
        public Guid UnitId { get; set; }
        public int BookingBeforeDate { get; set; } // Ngày cho phép đặt trước ngày hiệu lực
        public int BookingAfterDate { get; set; } // Ngày cho phép đặt sau ngày hiệu lực

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int ShiftCount { get; set; }

        public virtual ICollection<DoctorWorkingCalendar> DoctorCalendars { get; set; }
        public virtual ICollection<RoomWorkingCalendar> RoomCalendars { get; set; }
        public virtual ICollection<ServiceWorkingCalendar> ServiceCalendars { get; set; }
    }
}
