using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models.SMDModels
{
    public class SMDUnitBaseModel : BaseCodeModel
    {
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Website { get; set; }

        public string Introduction { get; set; }

        public bool IsTestingFacility { get; set; }
        public bool IsPrEPFacility { get; set; }
        public bool IsARTFacility { get; set; }

        public byte[] Logo { get; set; }
        [Required]
        public Guid UnitTypeId { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
    }

    public class SMDUnitCreateModel : SMDUnitBaseModel
    {
        [Required]
        public AccountCreateModel Account { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class SMDUnitUpdateModel : SMDUnitBaseModel
    {
        public Guid Id { get; set; }
    }

    public class SMDUnitViewModel : SMDUnitUpdateModel
    {
        public string Username { get; set; }
        public AllowInputType AllowInputType { get; set; }
    }
}
