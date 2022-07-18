using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class UnitCreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid UnitTypeId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Ward { get; set; }

        public string Website { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Introduction { get; set; }


    }

    public class UnitUpdateModel : UnitCreateModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }

    public class UnitModel : UnitUpdateModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public byte[] Logo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsTestingFacility { get; set; }
        public bool IsPrEPFacility { get; set; }
        public bool IsARTFacility { get; set; }
        public Guid? ParentId { get; set; }

    }

    public class HospitalUpdateLogo
    {
        public Guid Id { get; set; }
        public IFormFile Picture { get; set; }
    }

    public class HospitalUpdateImages
    {
        public Guid Id { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class UnitImagesModel : BaseEntity
    {
        public byte[] Image { get; set; }
    }

    public class FileModel
    {
        public Guid Id { get; set; }
        public string FileType { get; set; }
        public byte[] Data { get; set; }
    }


    public class SetTestingFacilityModel
    {
        public bool IsTestingFacility { get; set; }
    }
    public class SetPrEPFacilityModel
    {
        public bool IsPrEPFacility { get; set; }
    }
    public class SetARTFacilityModel
    {
        public bool IsARTFacility { get; set; }
    }

    public class CreateOrganizationModel : UnitCreateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
