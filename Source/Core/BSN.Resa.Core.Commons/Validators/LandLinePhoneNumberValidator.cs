
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    public class LandLinePhoneNumberValidator
    {
		public const int Length = 11;

		public static ValidationResult Validate(object value)
		{
			var number = value?.ToString() ?? "";

			//کد شهرستان عدد 11 رقمی می باشد که با 01 تا 08 شروع میشود
			if (!new Regex(@"^0[1-8][0-9]{" + (Length - 2) + @"}$", RegexOptions.IgnoreCase).IsMatch(number))
				return new ValidationResult(Locale.Resources.PhoneNumberInvalid);

			return ValidationResult.Success;
		}

		public static bool IsValid(object value)
		{
			return Validate(value) == ValidationResult.Success;
		}
	}
}
