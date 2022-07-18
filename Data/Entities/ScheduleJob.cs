using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class ScheduleJob
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        public Guid ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }
    }
}
