using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ServiceWorkingCalendarModel
    {
        public ServiceModel Service { get; set; }
    }

    public class ServiceCalendarReturnModel
    {
        public Guid ServiceId { get; set; }
    }
}
