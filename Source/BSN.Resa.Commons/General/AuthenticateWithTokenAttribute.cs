using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BSN.Resa.Commons
{
	public class AuthenticateWithTokenAttribute : Attribute, IAuthenticationFilter
	{
		public AuthenticateWithTokenAttribute(string controllerRoutePrefix, string[] configurationAuthroizedTokens, string[] literalAuthorizedTokens = null)
		{
			if (!controllerRoutePrefix.Contains("{token}"))
				throw new ArgumentException();

			_tokenRegex = new Regex(controllerRoutePrefix.Replace("{token}", "([\\w|-]+)"));

			_authroizedTokens = new List<string>();

			foreach (string token in configurationAuthroizedTokens)
				_authroizedTokens.Add(ConfigurationManager.AppSettings[token]);

			if (literalAuthorizedTokens != null)
				_authroizedTokens.AddRange(literalAuthorizedTokens);
		}

		public bool AllowMultiple => false;

		public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			HttpRequestMessage request = context.Request;
			string path = request.RequestUri.PathAndQuery;

			if (!_tokenRegex.IsMatch(path))
				context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
			else if (!_authroizedTokens.Contains(_tokenRegex.Match(path).Groups[1].Value))
				context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
			else
				context.Principal = new ClaimsPrincipal();

			return Task.FromResult(0);
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			return Task.FromResult(0);
		}

		private Regex _tokenRegex;
		private List<string> _authroizedTokens;
	}

	public class AuthenticationFailureResult : IHttpActionResult
	{
		public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
		{
			ReasonPhrase = reasonPhrase;
			Request = request;
		}

		public string ReasonPhrase { get; private set; }

		public HttpRequestMessage Request { get; private set; }

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute());
		}

		private HttpResponseMessage Execute()
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
			response.RequestMessage = Request;
			response.ReasonPhrase = ReasonPhrase;
			return response;
		}
	}
}
