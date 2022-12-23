using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class UrlValidator
    {
        public static ValidationResult Validate(object value)
        {
            string input = value?.ToString() ?? string.Empty;

            string pattern = @"https?://localhost";
            if (new Regex(pattern).IsMatch(input))
                return ValidationResult.Success;

            //https://regexr.com/39nr7
            //https://mathiasbynens.be/demo/url-regex
            pattern = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";

            if (new Regex(pattern).IsMatch(input))
                return ValidationResult.Success;

            return new ValidationResult(Locale.Resources.InputInvalid);
        }

        public static bool IsValid(object value)
        {
            return Validate(value) == ValidationResult.Success;
        }
    }
}
