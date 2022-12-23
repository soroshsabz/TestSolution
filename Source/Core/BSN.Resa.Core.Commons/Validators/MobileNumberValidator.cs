using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    //TODO: this regex will not suppor the international numbers
    //FIXME: support international numbers validation
    public class MobileNumberValidator
    {
        public const int Length = 11;

        public static ValidationResult ValidateMobileNumber(object value)
        {
            var number = value?.ToString() ?? "";

            if (!new Regex(@"^(0098|98|\+98|0|)9[0-9]{9}$", RegexOptions.IgnoreCase).IsMatch(number))
                return new ValidationResult(Locale.Resources.MobileNumberInvalid);

            return ValidationResult.Success;
        }
        public static bool IsValidMobileNumber(object value)
        {
            return ValidateMobileNumber(value) == ValidationResult.Success;
        }
    }
}