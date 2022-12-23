using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
    public class NationalNumberAttribute : ValidationAttribute
    {
        public bool AllowBlank { get; set; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return AllowBlank && string.IsNullOrEmpty(value?.ToString())
                ? ValidationResult.Success
                : NationalNumberValidator.Validate(value);
        }
    }
}
