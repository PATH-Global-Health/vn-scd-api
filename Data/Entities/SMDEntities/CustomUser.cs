using Data.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class CustomUser
    {
        public string Username { get; set; }
        public Role Role { get; set; }
        public Guid UnitId { get; set; }
    }
}
