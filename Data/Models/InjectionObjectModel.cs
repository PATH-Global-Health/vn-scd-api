using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class InjectionObjectAddModel
    {
        public string Name { get; set; }
        public int? FromDaysOld { get; set; }
        public int? ToDaysOld { get; set; }
    }
    public class InjectionObjectUpdateModel : InjectionObjectAddModel
    {
        public Guid Id { get; set; }
    }
    public class InjectionObjectViewModel : InjectionObjectUpdateModel
    {
    }

    public class InjectionObjectServiceTypeAddModel
    {
        public Guid InjectionObjectId { get; set; }
        public Guid ServiceTypeId { get; set; }
    }
}
