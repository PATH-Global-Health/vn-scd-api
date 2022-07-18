using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ProfileAddModel
    {
        [Required]
        public string Fullname { get; set; }
        public bool? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string VaccinationCode { get; set; }
        public string IdentityCard { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string PassportNumber { get; set; }
        public string Nation { get; set; }
        public int Status { get; set; }
    }

    public class AddProfileFacilityModel
    {
        [Required]
        public string Fullname { get; set; }
        [Required]
        public bool? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string VaccinationCode { get; set; }
        public string IdentityCard { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string PassportNumber { get; set; }
        public string Nation { get; set; }
        public int Status { get; set; }
        public Guid UnitId { get; set; }
    }

    public class ProfileUpdateModel : ProfileAddModel
    {
        public Guid Id { get; set; }
    }

    public class ProfileViewModel
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public bool Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string VaccinationCode { get; set; }
        public string IdentityCard { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }

        public Guid? RelationProfileId { get; set; }
        public string PassportNumber { get; set; }
        public string Nation { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }
        public string ExternalId { get; set; }

    }

    public class CustomerQR
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        public bool? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }

    public class CustomerIdentification: ProfileAddModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class StatusProfileModel
    {
        public Guid CustomerId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
