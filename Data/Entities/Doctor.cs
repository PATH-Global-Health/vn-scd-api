using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Entities
{
    public class Doctor : Person
    {
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string Title { get; set; }
        public string AcademicTitle { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public virtual ICollection<UnitDoctor> UnitDoctors { get; set; }

    //public Guid UserId { get; set; }
}
}
