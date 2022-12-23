using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using BSN.Resa.Locale;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class InternationalBankAccountNumberValidator
    {
        public const int MaximumLength = 34;

        public static ValidationResult Validate(object value)
		{
			var iBAN = value?.ToString() ?? "";

            //https://en.wikipedia.org/wiki/International_Bank_Account_Number#Structure
            if (!Regex.IsMatch(iBAN, @"^[a-zA-Z]{2}[0-9]{2}[0-9a-zA-Z]{1,30}$"))
                return new ValidationResult(Resources.IBANInvalid);

			return ValidationResult.Success;
		}

		public static bool IsValid(object value)
		{
			return Validate(value) == ValidationResult.Success;
		}
	}
}
