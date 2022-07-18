using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class ProjectBaseModel : BaseCodeModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public AllowInputType AllowInputType { get; set; }
    }


    public class ProjectCreateModel : ProjectBaseModel
    {
        [Required]
        public AccountCreateModel Account { get; set; }
    }

    public class ProjectUpdateModel : ProjectBaseModel
    {
        public Guid Id { get; set; }
    }

    public class ProjectViewModel : ProjectUpdateModel
    {
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class AccountCreateModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool HasSendInitialEmail { get; set; } = true;
    }

    public class AddAccountModel : AccountCreateModel
    {
        public Guid ProjectId { get; set; }
    }
}
