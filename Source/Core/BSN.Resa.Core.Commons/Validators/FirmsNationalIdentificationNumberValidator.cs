using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class FirmsNationalIdentificationNumberValidator
    {
        public const int Length = 11;

        public static ValidationResult Validate(object value)
        {
            string number = value?.ToString() ?? "";

            if (!new Regex(@"^[0-9]{" + Length + @"}$").IsMatch(number))
                return new ValidationResult(Locale.Resources.FirmsNationalIdentificationNumberInvalid);

            return ValidationResult.Success;
        }

        public static bool IsValid(object value)
        {
            return Validate(value) == ValidationResult.Success;
        }
    }
}
