using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public partial class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
        [MaxLength(1000, ErrorMessage = "'Description' Length Exceed 1000")]
        public string Description { get; set; }
    }

    public class BaseCodeEntity : BaseEntity
    {
        [MaxLength(16)]
        public string Code { get; set; }
        public bool BlockChanges { get; set; }
    }
}
