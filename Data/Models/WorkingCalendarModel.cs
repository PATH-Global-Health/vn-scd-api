using Data.Constants;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class WorkingCalendarCreateModel
    {
        public string Name { get; set; }
        public List<DayCreateModel> DayCreateModels { get; set; }
        public int Interval { get; set; }
        public int BookingBefore { get; set; }
        public int BookingAfter { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ShiftCount { get; set; }
        public Guid UnitId { get; set; }
        //public List<Guid> Rooms { get; set; }
        //public List<Guid> Doctors { get; set; }
        public List<DoctorRoom> DoctorRooms { get; set; }
        public List<Guid> Services { get; set; }
    }

    public class DoctorRoom
    {
        public Guid DoctorId { get; set; }
        public Guid RoomId { get; set; }
    }
    public class WorkingCalendarViewModel
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int BookingBeforeDate { get; set; }
        public int BookingAfterDate { get; set; }
        public string Description { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string FromTo { get; set; }

        public DoctorModel Doctor { get; set; }
        public RoomModel Room { get; set; }
        public List<ServiceWorkingCalendarModel> Services { get; set; }

        public WorkingCalendarStatus Status { get; set; }
    }

    public class DateDetailViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public WorkingCalendarStatus Status { get; set; }
        public FromToModel Schedules { get; set; } = new FromToModel();
        public BaseModel Doctor { get; set; }
        public BaseModel Room { get; set; }
        public string HospitalName { get; set; }
        public List<BaseModel> Service { get; set; }
    }
    public class IntervalViewModel
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public WorkingCalendarStatus Status { get; set; }
        public bool IsAvailable { get; set; }
        public int NumId { get; set; }
        public int AvailableQuantity { get; set; }
    }
    public class IntervelShiftViewModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public WorkingCalendarStatus Status { get; set; }
        public List<IntervalViewModel> Intervals { get; set; } = new List<IntervalViewModel>();
    }

    public class FromToModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
    public class WorkingCalendarCreateReturnModel
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime BookingBeforeDate { get; set; }
        public DateTime BookingAfterDate { get; set; }
        public string Description { get; set; }

        public ICollection<DoctorCalendarReturnModel> DoctorCalendars { get; set; }
        public ICollection<RoomCalendarReturnModel> RoomCalendars { get; set; }
        public ICollection<ServiceCalendarReturnModel> ServiceCalendars { get; set; }
    }
}
