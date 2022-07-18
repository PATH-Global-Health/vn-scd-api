using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Buffers.Text;
using Data.Entities.SMDEntities;
using Data.Entities.SMDEntities.SMDEntityInterfaces;
using Data.Constants;

namespace Data.Entities
{
    public class Unit : BaseCodeEntity, ICBO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Introduction { get; set; }

        public bool IsTestingFacility { get; set; }
        public bool IsPrEPFacility { get; set; }
        public bool IsARTFacility { get; set; }


        public byte[] Logo { get; set; }
        public Guid UnitTypeId { get; set; }
        [ForeignKey("UnitTypeId")]
        public virtual UnitType UnitType { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Unit ParentUnit { get; set; }

        #region SMD
        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

        public AllowInputType AllowInputType { get; set; }
        #endregion
    }
}
