using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Room : Place
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Guid UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }
    }
}
