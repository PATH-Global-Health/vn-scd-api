using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class LayTestCreateModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(4)]
        public string FacilityId { get; set; }
        [Required]
        [Range(1, Double.MaxValue)]
        public double HIVPublicExaminationDate { get; set; }
        [Required]
        public string PublicExaminationOrder { get; set; }
        [Required]
        public string PublicExaminationCode { get; set; }
        [Required]
        [Range(0, 2)]
        public int ExaminationForm { get; set; }
        [Required]
        [MaxLength(9)]
        public string ReceptionId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
    }
    public class PatientCreateModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, Double.MaxValue)]
        public double CreatedDate { get; set; }
        [Required]
        [MaxLength(4)]
        public string FacilityId { get; set; }
        [Required]
        [MaxLength(9)]
        public string ReceptionId { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public CustomerInfo CustomerInfo { get; set; }
    }

    public class CustomerInfo
    {
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public List<string> RelatedIds { get; set; }
    }
}
