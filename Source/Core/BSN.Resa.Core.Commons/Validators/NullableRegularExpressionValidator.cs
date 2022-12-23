using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
	public class NullableRegularExpressionAttribute : RegularExpressionAttribute
	{
		public NullableRegularExpressionAttribute(string pattern) : base(pattern)
		{ }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return string.IsNullOrEmpty(value?.ToString())
                ? ValidationResult.Success
                : base.IsValid(value, validationContext);
        }
    }
}
