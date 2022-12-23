using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace BSN.Resa.Commons
{
	// Visit https://blog.markvincze.com/how-to-validate-action-parameters-with-dataannotation-attributes/
	// This class is not duplicate of ValidateMvcActionParametersAttribute just the Class Names is similar to that.
	// See Namespaces which are used.
	// They just used for same purpose.
	public class ValidateWebApiActionParametersAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext context)
		{
			var descriptor = context.ActionDescriptor as ReflectedHttpActionDescriptor;

			if (descriptor != null)
			{
				var parameters = descriptor.MethodInfo.GetParameters();

				foreach (var parameter in parameters)
				{
					var argument = context.ActionArguments[parameter.Name];

					EvaluateValidationAttributes(parameter, argument, context.ModelState);
				}
			}

			base.OnActionExecuting(context);
		}

		private void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
		{
			var validationAttributes = parameter.CustomAttributes;

			foreach (var attributeData in validationAttributes)
			{
				var attributeInstance = parameter.GetCustomAttribute(attributeData.AttributeType);

				var validationAttribute = attributeInstance as ValidationAttribute;

				if (validationAttribute != null)
				{
					var isValid = validationAttribute.IsValid(argument);
					if (!isValid)
					{
						modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
					}
				}
			}
		}
	}
}