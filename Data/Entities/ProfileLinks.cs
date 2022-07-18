using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Data.Constants;

namespace Data.Entities
{
    public class ProfileLinks:BaseEntity
    {
        public TypeFacitily Type { get; set; }
        public Guid LinkTo { get; set; }
        public Guid ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

    }
}
