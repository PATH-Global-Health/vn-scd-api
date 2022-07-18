using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class DoctorCreateModel
    {
        [Required]
        [MaxLength(16)]
        public string Code { get; set; }

        [Required]
        [MaxLength(256)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(16)]
        public string IdentityCard { get; set; }

        //[Required]
        [MaxLength(256)]
        public string Title { get; set; }

        //[Required]
        [MaxLength(256)]
        public string AcademicTitle { get; set; }

        [Required]
        public bool Gender { get; set; }

        //[EmailAddress]
        //[Required(AllowEmptyStrings = true)]
        public string Email { get; set; }
        //[Phone]
        public string Phone { get; set; }


    }

    public class DoctorUpdateModel : DoctorCreateModel
    {
        public Guid Id { get; set; }
    }

    public class DoctorModel : DoctorUpdateModel
    {
        public bool IsDeleted { get; set; }
    }

    public class DoctorViewModel : DoctorCreateModel
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public List<UnitModel> Unit { get; set; }
    }

    public class RegisterDoctorModel : DoctorCreateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
       
    }

}
