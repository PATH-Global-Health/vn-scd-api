using System;
using System.Collections.Generic;
using System.Text;
using Data.Constants;

namespace Data.Models
{
    public class DayAddModel
    {
        public DateTime Date { get; set; }
        public List<ScheduleCreateModel> Schedules { get; set; }
    }

    public class DayCreateModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public List<DateTime> Date { get; set; }
        //public List<ScheduleCreateModel> Schedules { get; set; }
    }

    public class DayViewModel
    {
        public DateTime Date { get; set; }
        public List<WorkingCalendarViewModel> WorkingCalendar { get; set; }

    }
}
