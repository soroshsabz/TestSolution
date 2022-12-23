using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
    public class ChargeAmountAttribute : ValidationAttribute
    {
        public bool AllowBlank { get; set; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ChargeAmountValidator.ValidateAmount(value);
        }
    }
}