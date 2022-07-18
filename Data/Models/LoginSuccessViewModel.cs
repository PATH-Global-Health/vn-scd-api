using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class LoginSuccessViewModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public object Token { get; set; }
    }
}
