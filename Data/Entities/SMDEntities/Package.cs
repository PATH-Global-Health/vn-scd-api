using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Entities.SMDEntities
{
    public class Package : BaseCodeEntity
    {
        public string Name { get; set; }
    }
}
