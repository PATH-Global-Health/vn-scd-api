using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Data.Constants;
using Data.Entities;

namespace Data.Models
{
    public class ProfileLinkModel:BaseModel
    {
        public TypeFacitily Type { get; set; }
        public Guid LinkTo { get; set; }
        public Guid ProfileId { get; set; }
    }

    public class ProfileLinkViewModel: ProfileLinkModel
    {

    }
}
