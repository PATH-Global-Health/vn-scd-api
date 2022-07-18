using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class UserCreateModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string FullName { get; set; } = "";
        public bool IsEmailConfirmed { get; set; } = true;
        public string GroupName { get; set; }
        public bool OnlyUsername { get; set; } = false;
    }

    public class CBOCreateModel : UserCreateModel
    {
        public bool HasSendInitialEmail { get; set; } = true;
    }
}
