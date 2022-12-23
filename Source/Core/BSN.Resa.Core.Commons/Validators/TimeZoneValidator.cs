using System;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class TimeZoneValidator
    {
        public static ValidationResult Validate(object value)
        {
            string input = value?.ToString() ?? string.Empty;

            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(input);
            }
            catch (Exception)
            {
                return new ValidationResult(Locale.Resources.InputInvalid);
            }

            return ValidationResult.Success;
        }

        public static bool IsValid(object value)
        {
            return Validate(value) == ValidationResult.Success;
        }
    }
}
