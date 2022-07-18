using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class RegisterModel
    {
        #region Identity Info
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        #endregion

        #region User Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        #endregion
    }

    public class HospitalRegister : UnitCreateModel
    {
        //public string Username { get; set; }
        //public string Password { get; set; }
    }
}
