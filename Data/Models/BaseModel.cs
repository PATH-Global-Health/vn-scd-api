using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }

    public class ErrorMessage
    {
        public string Validate { get; set; }
    }

    public class BaseCodeModel
    {
        [Required]
        [StringLength(16)]
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
