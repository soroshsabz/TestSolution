using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
	public class RequiredIfAttribute : ValidationAttribute
	{
		private string _propertyName { get; set; }

		private object _desiredValue { get; set; }

		private readonly RequiredAttribute _innerAttribute;

		public RequiredIfAttribute(string propertyName, object desiredvalue)
		{
			_propertyName = propertyName;
			_desiredValue = desiredvalue;
			_innerAttribute = new RequiredAttribute();
		}

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            //var dependentValue = context.ObjectInstance.GetType().GetProperty(_propertyName).GetValue(context.ObjectInstance, null);

            //dependentValue = dependentValue == null ? string.Empty : string.IsNullOrWhiteSpace(dependentValue.ToString()) ? string.Empty : dependentValue.ToString();
            //string desiredValue = _desiredValue == null ? string.Empty : string.IsNullOrWhiteSpace(_desiredValue.ToString()) ? string.Empty : _desiredValue.ToString();

            //if ((string)dependentValue == desiredValue && !_innerAttribute.IsValid(value))
            //    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });

            return ValidationResult.Success;
        }
    }

}
