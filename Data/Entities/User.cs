using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities
{
    public class User:IdentityUser
    {
        public string FullName { get; set; }
    }
}
