using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace BSN.Resa.Commons
{
	public class AuthenticateActionWithTokenAttribute : ActionFilterAttribute
	{

		public AuthenticateActionWithTokenAttribute(string routeParamName, string tokenConfigKey)
		{
			_routeParamName = routeParamName;
			_tokenValue = ConfigurationManager.AppSettings[tokenConfigKey];
		}

		public override void OnActionExecuting(HttpActionContext context)
		{
			string token = context.Request.GetRouteData().Values[_routeParamName] as string;
			if (token == null || token != _tokenValue)
				context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized); ;
			base.OnActionExecuting(context);
		}

		private readonly string _routeParamName;
		private readonly string _tokenValue;
	}
}