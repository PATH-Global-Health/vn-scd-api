using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Profile : BaseEntity
    {
        public string ExternalId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public bool? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string VaccinationCode { get; set; } // bỏ
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string IdentityCard { get; set; }
        public string PassportNumber { get; set; }
        public string Nation { get; set; }
        public int Status { get; set; }
        public string SentFrom { get; set; }
        public string ReceptionId { get; set; }
        public string EmployeeId { get; set; }
        public Guid? RelationProfileId { get; set; }
        [ForeignKey("RelationProfileId")]
        public virtual Profile RelationProfile { get; set; }
    }
}
