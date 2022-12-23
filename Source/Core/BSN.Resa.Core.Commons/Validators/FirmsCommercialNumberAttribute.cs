using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
    public class FirmsCommercialNumberAttribute : ValidationAttribute
    {
        public bool AllowBlank { get; set; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return AllowBlank && string.IsNullOrEmpty(value?.ToString())
                ? ValidationResult.Success
                : FirmsCommercialNumberValidator.Validate(value);
        }
    }
}
