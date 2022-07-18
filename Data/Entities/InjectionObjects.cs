using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities
{
    public class InjectionObject : BaseEntity
    {
        public string Name { get; set; }
        public int? FromDaysOld { get; set; }
        public int? ToDaysOld { get; set; }
    }
}
