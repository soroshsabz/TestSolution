using BSN.Resa.Core.Commons.ViewModels;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using JsonException = Newtonsoft.Json.JsonException;
using BSN.Resa.DoctorApp.Data.Infrastructure;

namespace BSN.Resa.DoctorApp.Data.ServiceCommunicators.ApplicationServiceCommunicator
{
    public class ApplicationServiceCommunicator : IApplicationServiceCommunicator
    {
        #region private static fields

        private const int AppUpdateManifestFetchingTimeoutInSeconds = 10;

        private const int AuthenticationTimeoutInSeconds = 30;

        #endregion

        public ApplicationServiceCommunicator(
            ConnectionStatusManager connectionStatusManager,
            IConnectivity connectivity,
            IConfig config,
            INativeHttpMessageHandlerProvider nativeHttpMessageHandlerProvider)
		{
            _connectionStatusManager = connectionStatusManager;
            _connectivity = connectivity;
            _config = config;
			_nativeHttpMessageHandlerProvider = nativeHttpMessageHandlerProvider;
		}

        public async Task<IServiceResult<AppUpdateManifest>> GetAppUpdateManifestAsync()
        {
            var result = new GeneralServiceResult<AppUpdateManifest>();

            if (!IsConnectivityAvailable())
            {
                //Note: ViewModels should check connectivity too. So this code shouldn't be returned in normal situations.
                result.ResultCode = ServiceResultCode.NoConnectivity;
                return result;
            }

            using (var handler = _nativeHttpMessageHandlerProvider.Get())
			{
				using (var client = new HttpClient(handler))
				{
					client.DefaultRequestHeaders.ConnectionClose = true; //to fix SocketException/EOFException
					client.BaseAddress = new Uri(_config.ResaDoctorAppApiAddress);
					client.Timeout = TimeSpan.FromSeconds(AppUpdateManifestFetchingTimeoutInSeconds);
					HttpResponseMessage response;

					_connectionStatusManager.StartConnection();
					try
					{
						response = await client
							.GetAsync($"UpdateManifest?platform={_config.TargetPlatform}&version={_config.Version}")
							.ConfigureAwait(false);
					}
					catch (Exception e) when (e is TaskCanceledException || e is SocketException) //TODO: handle EOFileException which is a native Java exception
					{
						result.ResultCode = ServiceResultCode.NetworkProblem;
						return result;
					}
					catch (Exception)
					{
						//See: https://stackoverflow.com/questions/40677708/which-exceptions-can-httpclient-throw

						result.ResultCode = ServiceResultCode.AppInternalProblem;
						return result;
					}
					_connectionStatusManager.EndConnection();

					result.HttpStatusCode = response.StatusCode;

					switch (response.StatusCode)
					{
						case HttpStatusCode.OK:
							{
								try
								{
									var responseContentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
									var appUpdateManifest = JsonConvert.DeserializeObject<AppUpdateManifest>(responseContentString);
									result.Data = appUpdateManifest;
									result.ResultCode = ServiceResultCode.Success;
									return result;
								}
								catch (JsonException)
								{
									result.ResultCode = ServiceResultCode.JsonException;
									return result;
								}
								catch (Exception)
								{
									result.ResultCode = ServiceResultCode.AppInternalProblem;
									return result;
								}
							}
						case HttpStatusCode.Forbidden:
						case HttpStatusCode.BadRequest:
						case HttpStatusCode.Unauthorized:
							{
								result.ResultCode = ServiceResultCode.AuthenticationFailed;
								return result;
							}
						case HttpStatusCode.RequestTimeout:
							{
								result.ResultCode = ServiceResultCode.NetworkProblem;
								return result;
							}
						default:
							{
								result.ResultCode = ServiceResultCode.AppInternalProblem;
								return result;
							}
					}
				}
			}
        }

		public async Task<OauthToken> AuthenticateAsync(string username, string password)
        {
            if (!IsConnectivityAvailable())
            {
                //Note: ViewModels should check connectivity too. So this code shouldn't be returned in normal situations.
                throw new InternetNotAvailableException();
            }

            using (var handler = _nativeHttpMessageHandlerProvider.Get())
			{
				using (var client = new HttpClient(handler))
				{
					client.DefaultRequestHeaders.ConnectionClose = true; //to fix SocketException/EOFException
					client.BaseAddress = new Uri(_config.ResaMobileApiAddress);

					var content = new FormUrlEncodedContent(new[]
					{
						new KeyValuePair<string, string>("username", username),
						new KeyValuePair<string, string>("password", password),
						new KeyValuePair<string, string>("grant_type", "password")
					});

					client.Timeout = TimeSpan.FromSeconds(AuthenticationTimeoutInSeconds);
					HttpResponseMessage response;

					_connectionStatusManager.StartConnection();

					try
					{
						response = await client.PostAsync($"{_config.ResaMobileApiToken}/oauth/token", content).ConfigureAwait(false);
					}
					catch (Exception e) when (e is TaskCanceledException || e is SocketException)
					{
						throw new NetworkConnectionException();
					}

					_connectionStatusManager.EndConnection();

					switch (response.StatusCode)
					{
						case HttpStatusCode.OK:
							{
								try
								{
									var serializerSettings = new JsonSerializerSettings
									{
										ContractResolver = new OauthTokenContractResolver()
									};
									var token = JsonConvert.DeserializeObject<OauthToken>(await response.Content.ReadAsStringAsync(), serializerSettings);

									return token;
								}
								catch (JsonException)
								{
									throw new ResaJsonException();
								}
							}
						case HttpStatusCode.Forbidden:
						case HttpStatusCode.BadRequest:
						case HttpStatusCode.Unauthorized:
							{
								throw new AuthenticationException();
							}
						case HttpStatusCode.RequestTimeout:
							{
								throw new NetworkConnectionException();
							}
						default:
							{
								throw new Exception();
							}
					}
				}
			}
        }

        private bool IsConnectivityAvailable()
        {
            return _connectivity.IsConnected;
        }

        private class OauthTokenContractResolver : DefaultContractResolver
        {
            private Dictionary<string, string> PropertyMappings { get; set; }

            public OauthTokenContractResolver()
            {
                PropertyMappings = new Dictionary<string, string>
                {
                    {nameof(OauthToken.AccessToken), "access_token"},
                    {nameof(OauthToken.ExpiresIn), "expires_in"},
                    {nameof(OauthToken.TokenType), "token_type"}
                };
            }

            protected override string ResolvePropertyName(string propertyName)
            {
                var resolved = PropertyMappings.TryGetValue(propertyName, out var resolvedName);
                return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
            }
        }

        #region Fields
        private readonly ConnectionStatusManager _connectionStatusManager;
        private readonly IConnectivity _connectivity;
        private readonly IConfig _config;
		private readonly INativeHttpMessageHandlerProvider _nativeHttpMessageHandlerProvider;
		#endregion
	}
}
