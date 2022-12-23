using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class NationalNumberValidator
	{
		public const int Length = 10;

		public static ValidationResult Validate(object value)
		{
			string number = value?.ToString() ?? "";
			
			if (!new Regex(@"^[0-9]{" + Length + @"}$").IsMatch(number) || new Regex(@"^(.)\1*$").IsMatch(number))
				return new ValidationResult(Locale.Resources.InputInvalid);

			var check = int.Parse(number.ToCharArray()[9].ToString());
			var sum = Enumerable.Range(0, 9).Sum(x => int.Parse(number.ToCharArray()[x].ToString()) * (10 - x));
			var remainder = sum % 11;

			if ((remainder < 2 && check == remainder) || (remainder >= 2 && check == 11 - remainder))
				return ValidationResult.Success;

			return new ValidationResult(Locale.Resources.InputInvalid);
		}

		public static bool IsValid(object value)
		{
			return Validate(value) == ValidationResult.Success;
		}
	}
}
