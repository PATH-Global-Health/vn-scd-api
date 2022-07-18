using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class KPICreateModel : IValidatableObject
    {
        [Required]
        public double From { get; set; }
        [Required]
        public double To { get; set; }
        [Required]
        public string Color { get; set; }
        public string Description { get; set; }

        public Guid IndicatorId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (From > To)
            {
                yield return new ValidationResult("'From' cannot be greater than 'To'", new[] { "From" });
            }
        }
    }

    public class KPIUpdateModel : KPICreateModel
    {
        public Guid Id { get; set; }
    }

    public class KPIViewModel : KPIUpdateModel { }
}
