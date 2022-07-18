using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class UserInformationViewModel : UserInformationModel
    {
        public string Username { get; set; }
    }
    public class UserInformationModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Introduction { get; set; }
    }
}
