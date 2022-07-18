using Data.Models.SMDModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Data.Models.CustomModels
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        public bool IgnoreCase { get; set; } = false;
        public bool AllowNullOrEmpty { get; set; } = false;

        public override bool IsValid(object value)
        {
            var comparer = IgnoreCase? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            if (AllowNullOrEmpty && string.IsNullOrEmpty(value?.ToString()))
            {
                return true;
            };
            if (AllowableValues?.Contains(value?.ToString(), comparer) == true)
            {
                return true;
            }
            Console.WriteLine("String range: " + value?.ToString());
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                return base.FormatErrorMessage(name);
            return $"({name}) only accepts values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
        }
    }

    public class DateInReportingPeriodAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateInReportingPeriodAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var indicator = (IndicatorReportModel)value;
            if (indicator == null && indicator.ReportDate == null)
                return ValidationResult.Success;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException($"Property {_comparisonProperty} not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);
            if (comparisonValue.Month != indicator.ReportDate.Value.Month || comparisonValue.Year != indicator.ReportDate.Value.Year)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.MemberName));
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                return base.FormatErrorMessage(name);
            return $"Date of ({name}) is not in {_comparisonProperty}";
        }
    }
}
