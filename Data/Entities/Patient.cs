using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Patient : BaseEntity
    {
        public string UserId { get; set; }
        public double CreatedDate { get; set; }
        public string FacilityId { get; set; }
        public string ReceptionId { get; set; }
        public string EmployeeId { get; set; }
        public string ExternalId { get; set; }

        //Customer infor
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
    }

    public class RelatedPatient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string RelatedId { get; set; }
    }

    public class LayTest
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public string FacilityId { get; set; }
        public double HIVPublicExaminationDate { get; set; }
        public string PublicExaminationOrder { get; set; }
        public string PublicExaminationCode { get; set; }
        public int ExaminationForm { get; set; }
        public string ReceptionId { get; set; }
        public string EmployeeId { get; set; }

        public Guid PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
    }
}
