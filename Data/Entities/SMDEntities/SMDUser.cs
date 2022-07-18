using Data.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class SMDUser : BaseEntity
    {
        public Guid UnitId { get; set; }
        public string Username { get; set; }
    }
}
