using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    /// <summary>
    /// For more information visit https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.100).aspx
    /// </summary>
    public abstract class EmailValidator
    {
        public static ValidationResult Validate(object value)
        {
            string input = value?.ToString() ?? string.Empty;

            if (!Regex.IsMatch(input, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
              @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase))
                return new ValidationResult(Locale.Resources.EmailInvalid);

            return ValidationResult.Success;
        }

        public static bool IsValid(object value)
        {
            return Validate(value) == ValidationResult.Success;
        }
    }
}
