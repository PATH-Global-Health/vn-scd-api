using Data.Constants;
using Data.Entities.SMDEntities.SMDEntityInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class Project : BaseCodeEntity, ICBO
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Unit> Units { get; set; }
        public AllowInputType AllowInputType { get; set; }
    }
}
