using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace BSN.Resa.Commons
{
	// Visit https://blog.markvincze.com/how-to-validate-action-parameters-with-dataannotation-attributes/
	public class ValidateMvcActionParametersAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			IMethodInfoActionDescriptor descriptor = context.ActionDescriptor as ReflectedActionDescriptor;
			descriptor = descriptor ?? context.ActionDescriptor as TaskAsyncActionDescriptor;

			if (descriptor != null)
			{
				var parameters = descriptor.MethodInfo.GetParameters();

				foreach (var parameter in parameters)
				{
					var argument = context.ActionParameters[parameter.Name];

					EvaluateValidationAttributes(parameter, argument, context.Controller.ViewData.ModelState);
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