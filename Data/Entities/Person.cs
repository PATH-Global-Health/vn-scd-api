using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Person : BaseEntity
    {
        public string Code { get; set; }
        public string UserId { get; set; }
    }
}
