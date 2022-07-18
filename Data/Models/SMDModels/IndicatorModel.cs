using Data.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data.Models
{
    public class IndicatorCreateModel : BaseCodeModel, IValidatableObject
    {
        [Required]
        public string Name { get; set; }
        public ReportValueType Type { get; set; }
        public Guid? NumeratorIndicatorId { get; set; }
        public Guid? DenominatorIndicatorId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (Type == ReportValueType.PERCENTAGE && (!NumeratorIndicatorId.HasValue || !DenominatorIndicatorId.HasValue))
                results.Add(new ValidationResult("Numerator and Denominator is required for calculation."));
            return results;
        }
    }

    public class IndicatorUpdateModel : IndicatorCreateModel
    {
        public Guid Id { get; set; }
        
    }
    public class IndicatorViewModel : IndicatorUpdateModel
    {
        public bool BlockChanges { get; set; }
    }
}
