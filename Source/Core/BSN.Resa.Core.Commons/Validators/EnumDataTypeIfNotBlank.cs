using System;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
	public class EnumDataTypeIfNotBlank : ValidationAttribute
	{
		private readonly EnumDataTypeAttribute _innerAttribute;

		public EnumDataTypeIfNotBlank(Type enumType)
		{
			_innerAttribute = new EnumDataTypeAttribute(enumType);
		}

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			if (value == null || (value is string && (string)value == string.Empty))
				return ValidationResult.Success;

            //if (!_innerAttribute.IsValid(value))
            //    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });

            return ValidationResult.Success;
		}
	}
}
