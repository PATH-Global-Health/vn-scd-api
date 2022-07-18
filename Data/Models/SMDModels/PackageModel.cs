using Data.Models.SMDModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class PackageCreateModel : BaseCodeModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class PackageUpdateModel : PackageCreateModel
    {
        public Guid Id { get; set; }
    }

    public class PackageViewModel : PackageUpdateModel
    {
        public bool BlockChanges { get; set; }
    }

    public class ImplementPackageCreateModel
    {
        [Required]
        public string Province { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public double TotalAmount { get; set; }
        public string Description { get; set; }
        public Guid PackageId { get; set; }
    }

    public class ImplementPackageUpdateModel : ImplementPackageCreateModel
    {
        public Guid Id { get; set; }
    }

    public class ImplementPackageViewModel : ImplementPackageUpdateModel
    {
        public string PackageCode { get; set; }

        public IEnumerable<TargetViewModel> Targets { get; set; }
    }
}
