using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class LatinInputValidator
    {
        public static ValidationResult Validate(object value)
        {
            string input = value?.ToString() ?? string.Empty;
            
            if (!new Regex(@"^[\u0000-\u024F]*$").IsMatch(input))
                return new ValidationResult(Locale.Resources.InputNotLatin);

            return ValidationResult.Success;
        }

        public static bool IsValid(object value)
        {
            return Validate(value) == ValidationResult.Success;
        }
    }
}
