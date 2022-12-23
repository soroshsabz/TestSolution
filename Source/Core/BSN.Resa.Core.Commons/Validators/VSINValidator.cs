using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class VSINValidator
	{
		public static ValidationResult Validate(object value)
		{
            string stringValue = value?.ToString() ?? "";
            long.TryParse(stringValue, out long vSIN);

            //3 digit to future use
            if (vSIN >= 1000 & vSIN <= 9999999)
                return ValidationResult.Success;

            return new ValidationResult(Locale.Resources.VSINInvalid);
		}

		public static bool IsValid(object value)
		{
			return Validate(value) == ValidationResult.Success;
		}
	}
}
