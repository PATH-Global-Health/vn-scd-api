using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class PersonPlace
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }

        public Guid PlaceId { get; set; }
        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }
    }
}
