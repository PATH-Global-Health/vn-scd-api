using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models.SMDModels
{
    public class ContractCreateModel
    {
        [Required]
        public Guid CBOId { get; set; }
        [Required]
        public Guid IPackageId { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }

    public class SetContractModel
    {
        public Guid ContractId { get; set; }
    }

    public class ContractUpdateModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }

    public class ContractViewModel : ContractCreateModel
    {
        public Guid Id { get; set; }
        public bool IsCurrent { get; set; }
        public double TotalAmount { get; set; }
        public string PackageName { get; set; }
        public ICollection<TargetViewModel> Targets { get; set; }
    }
}
