using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.MedicalTestViewModels;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using Newtonsoft.Json;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonException = Newtonsoft.Json.JsonException;

namespace BSN.Resa.DoctorApp.Data.ServiceCommunicators.DoctorServiceCommunicator
{
    /// <summary>
    /// Chain of responsibility pattern.
    /// For more info visit: https://refactoring.guru/design-patterns/chain-of-responsibility/csharp/example
    /// Why we use this pattern?:
    /// In the app for these two features: changing doctor's status and sending
    /// blocked numbers we have two provided APIs. One is a web REST API, and the other is an SMS server.
    /// We use SMS API for when Internet is not available in user's device.
    /// having said that, we use this pattern in such a way that first the REST API is used and if any probable
    /// problem occurs (such as if Internet is not available) then we use SMS API.
    /// </summary>
    public abstract class AbstractDoctorServiceCommunicatorHandler : IDoctorServiceCommunicator
    {
        #region Abstract Members

        public abstract Task SendDoctorStateAsync(DoctorState doctorState);

        public abstract Task SendBlockedPhoneNumbersAsync(string[] phoneNumbers);

        public abstract event OnSendDoctorStateResult OnSendDoctorStateResult;

        #endregion

        #region Constructor

        protected AbstractDoctorServiceCommunicatorHandler(
            IConnectivity connectivity,
            IConfig config,
            ConnectionStatusManager connectionStatusManager,
            INativeHttpMessageHandlerProvider nativeHttpMessageHandlerProvider)
        {
            _connectivity = connectivity;
            Config = config;
            _connectionStatusManager = connectionStatusManager;
            _nativeHttpMessageHandlerProvider = nativeHttpMessageHandlerProvider;
        }

        #endregion

        #region Protected Stuff

        protected IDoctorServiceBaseCommunicator NextBaseCommunicator;

        protected async Task<HttpContent> SendRequestAsync(string query, HttpMethod method, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync<object>(query, method, null, cancellationToken).ConfigureAwait(false);
        }

        protected async Task<HttpContent> SendRequestAsync<T>(string query, HttpMethod method, T content, CancellationToken cancellationToken = default)
        {
            CheckNetworkConnection();

            if (CommunicationToken == null)
            {
                _connectionStatusManager.SetAuthenticationStatus(AuthenticationStatus.Unauthorized);
                throw new AuthenticationException();
            }

            using (var handler = _nativeHttpMessageHandlerProvider.Get())
			{
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ConnectionClose = true; //to fix SocketException/EOFException
                    client.BaseAddress = new Uri(Config.ResaMobileApiAddress);

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(CommunicationToken.OauthToken.TokenType,
                            CommunicationToken.OauthToken.AccessToken);
                    client.Timeout = TimeSpan.FromSeconds(TIME_OUT);
                    HttpResponseMessage response;

                    _connectionStatusManager.StartConnection();
                    try
                    {
                        HttpRequestMessage requestMessage =
                            new HttpRequestMessage(method, new Uri(client.BaseAddress + query));
                        if (content != null)
                            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8,
                                "application/json");
                        response = await client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                        when (exception is TaskCanceledException || exception is SocketException)
                    {
                        response = new HttpResponseMessage(HttpStatusCode
                            .RequestTimeout); //TODO: TaskCanceledException does not mean RequestTimeout!!!
                    }

                    _connectionStatusManager.EndConnection();

                    if (response.IsSuccessStatusCode)
                        return response.Content;

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                        case HttpStatusCode.Forbidden:
                            {
                                _connectionStatusManager.SetAuthenticationStatus(AuthenticationStatus.Unauthorized);
                                throw new AuthenticationException();
                            }
                        case HttpStatusCode.RequestTimeout:
                            throw new NetworkConnectionException();
                    }

                    throw new ServiceCommunicationException();
                }
			}
        }

        protected readonly IConfig Config;

        #endregion

        #region Chain of Responsibility Members

        public IDoctorServiceBaseCommunicator SetNextCommunicator(IDoctorServiceBaseCommunicator nextBaseCommunicator)
        {
            NextBaseCommunicator = nextBaseCommunicator;
            return nextBaseCommunicator;
        }

        public DoctorServiceCommunicationToken CommunicationToken
        {
            get => _communicationToken;
            set
            {
                _communicationToken = value;
                if (NextBaseCommunicator != null)
                {
                    NextBaseCommunicator.CommunicationToken = value;
                }
            }
        }

        #endregion

        #region IDoctorServiceCommunicator Implementations

        public async Task<DoctorState> GetDoctorStateAsync(CancellationToken cancellationToken = default)
        {
            HttpContent responseContent = await SendRequestAsync("Doctors/State", HttpMethod.Get, cancellationToken)
                .ConfigureAwait(false);

            return (DoctorState)int.Parse(await responseContent.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<string[]> GetBlockedPhoneNumbersAsync()
        {
            HttpContent responseContent =
                await SendRequestAsync("Doctors/BlockedConsumers", HttpMethod.Get).ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<string[]>(await responseContent.ReadAsStringAsync())
                    .ToE164PhoneNumberFormat();
            }
            catch (JsonException exception)
            {
                throw new ResaJsonException(exception.Message);
            }
        }

        public async Task<string[]> GetAllowedPhoneNumbersAsync()
        {
            HttpContent responseContent =
                await SendRequestAsync("Doctors/AllowedNumbers", HttpMethod.Get).ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<string[]>(await responseContent.ReadAsStringAsync())
                    .ToE164PhoneNumberFormat();
            }
            catch (JsonException exception)
            {
                throw new ResaJsonException(exception.Message);
            }
        }

        public async Task SendAllowedPhoneNumbersAsync(string[] phoneNumbers)
        {
            await SendRequestAsync("Doctors/AllowedNumbers", HttpMethod.Post,
                phoneNumbers.ToNationalPhoneNumberFormat()).ConfigureAwait(false);
        }

        public async Task RemoveBlockedPhoneNumbersAsync(string[] phoneNumbers)
        {
            foreach (var phoneNumber in phoneNumbers)
                await SendRequestAsync($"Doctors/BlockedConsumers/{phoneNumber.ToNationalPhoneNumberFormat()}",
                    HttpMethod.Delete).ConfigureAwait(false);
        }

        public async Task RemoveAllowedPhoneNumbersAsync(string[] phoneNumbers)
        {
            foreach (var phoneNumber in phoneNumbers)
                await SendRequestAsync($"Doctors/AllowedNumbers/{phoneNumber.ToNationalPhoneNumberFormat()}",
                    HttpMethod.Delete).ConfigureAwait(false);
        }

        public async Task<string[]> GetResaPhoneNumbersAsync()
        {
            HttpContent responseContent =
                await SendRequestAsync("ResaPhoneNumbers", HttpMethod.Get).ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<string[]>(await responseContent.ReadAsStringAsync())
                    .ToE164PhoneNumberFormat();
            }
            catch (JsonException exception)
            {
                throw new ResaJsonException(exception.Message);
            }
        }

        public async Task<DoctorPreviewViewModel> GetDoctorPreviewAsync(CancellationToken cancellationToken = default)
        {
            HttpContent responseContent =
                await SendRequestAsync("Doctors/Preview", HttpMethod.Get, cancellationToken).ConfigureAwait(false);
            try
            {
                return
                    JsonConvert.DeserializeObject<DoctorPreviewViewModel>(await responseContent.ReadAsStringAsync()
                        .ConfigureAwait(false));
            }
            catch (JsonException exception)
            {
                throw new ResaJsonException(exception.Message);
            }
        }

        public async Task<ICollection<CallbackRequestPreview>> GetAllCallbackRequestsAsync(int offset = 0, int limit = int.MaxValue, CancellationToken cancellationToken = default)
        {
            var result = await SendRequestV2Async<ICollection<CallbackRequestPreview>>
                ($"Doctors/CallbackRequests?offset={offset}&limit={limit}", HttpMethod.Get, cancellationToken);
            return result;
        }

        public async Task<ICollection<CallbackRequestPreview>> GetActiveCallbackRequestsAsync(int offset = 0, int limit = Int32.MaxValue, CancellationToken cancellationToken = default)
        {
            var result = await SendRequestV2Async<ICollection<CallbackRequestPreview>>
                ($"Doctors/CallbackRequests/Active?offset={offset}&limit={limit}", HttpMethod.Get, cancellationToken);
            return result;
        }

        public async Task BookCallbackRequestAsync(string callbackId, CancellationToken cancellationToken = default)
        {
            await SendRequestV2Async($"Doctors/CallbackRequests/{callbackId}/Booking", HttpMethod.Post,
                cancellationToken);
        }

        public async Task<string> GetCallbackRequestPhoneNumber(CancellationToken cancellationToken = default)
        {
            var result = await SendRequestV2Async<string>("CallbackRequestPhoneNumber", HttpMethod.Get, cancellationToken);
            return result;
        }

        public async Task<ICollection<MedicalTestViewModel>> GetAllActiveMedicalTests(int offset = 0, int limit = int.MaxValue, CancellationToken cancellationToken = default)
        {
            var result = await SendRequestV2Async<MedicalTestRootViewModel>($"MedicalTests/Active", HttpMethod.Get, cancellationToken);
            return result?.Data;
        }

        public async Task SubmitMedicalTestTextReply(int medicalTestId, string reply, CancellationToken cancellationToken = default)
        {
            var content = new MedicalTestReplyTextViewModel
            {
                Msg = new MedicalTestReplyTextDateViewModel
                {
                    Text = reply
                }
            };

            await SendRequestV2Async($"MedicalTests/{medicalTestId}/ReplyText", HttpMethod.Post, content, cancellationToken: cancellationToken);
        }

        public async Task SubmitMedicalTestVoiceReply(int medicalTestId, FileStream voiceFileStream, string voiceFileNameWithExtension, CancellationToken cancellationToken = default)
        {
            var streamContent = new StreamContent(voiceFileStream);
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            streamContent.Headers.ContentDisposition.Name = "\"voice\"";
            streamContent.Headers.ContentDisposition.FileName = $"\"{voiceFileNameWithExtension}\"";
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            string boundary = Guid.NewGuid().ToString();

            var content = new MultipartFormDataContent(boundary);
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            content.Add(streamContent);

            await SendRequestV2Async($"MedicalTests/{medicalTestId}/ReplyVoice", HttpMethod.Post, content, TimeSpan.FromMinutes(30), cancellationToken: cancellationToken);
        }

        #endregion

        #region Private Methods

        private void CheckNetworkConnection()
        {
            if (!_connectivity.IsConnected)
                throw new InternetNotAvailableException();
        }

        private async Task<TResultDataType> SendRequestV2Async<TInputType, TResultDataType>(string query,
            HttpMethod method, TInputType content, TimeSpan timeOut = default, CancellationToken cancellationToken = default)
        {
            if (CommunicationToken == null)
            {
                _connectionStatusManager.SetAuthenticationStatus(AuthenticationStatus.Unauthorized);
                throw new AuthenticationException();
            }

            using (var handler = _nativeHttpMessageHandlerProvider.Get())
			{
				using (var client = new HttpClient(handler))
				{
					client.DefaultRequestHeaders.ConnectionClose = true; //to fix SocketException/EOFException
					client.BaseAddress = new Uri(Config.ResaMobileApiAddress);

					client.DefaultRequestHeaders.Authorization =
						new AuthenticationHeaderValue(CommunicationToken.OauthToken.TokenType,
							CommunicationToken.OauthToken.AccessToken);

					if (timeOut != default)
					{
						client.Timeout = timeOut;
					}
					else
					{
						client.Timeout = TimeSpan.FromSeconds(TIME_OUT);
					}

					HttpResponseMessage response;

					_connectionStatusManager.StartConnection();
					try
					{
						HttpRequestMessage requestMessage =
							new HttpRequestMessage(method, new Uri(client.BaseAddress + query));

						if (content != null)
						{
							if (content is HttpContent httpContentType)
							{
								requestMessage.Content = httpContentType;
							}
							else
							{
								requestMessage.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
							}
						}

						response = await client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
					}
					catch (JsonException jsonException)
					{
						throw new ResaJsonException(jsonException.Message);
					}
					catch (OperationCanceledException)
					{
						throw new NetworkConnectionException(isOperationCancelled: true);
					}
					catch (SocketException)
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
									var data = JsonConvert.DeserializeObject<TResultDataType>(await response.Content.ReadAsStringAsync());
									return data;
								}
								catch (JsonException e)
								{
									throw new ResaJsonException(e.Message);
								}
							}
						case HttpStatusCode.Forbidden:
						case HttpStatusCode.Unauthorized:
							{
								_connectionStatusManager.SetAuthenticationStatus(AuthenticationStatus.Unauthorized);
								throw new AuthenticationException();
							}
						case HttpStatusCode.PaymentRequired:
							{
								throw new PaymentRequiredException();
							}
						case HttpStatusCode.RequestTimeout:
							{
								throw new NetworkConnectionException();
							}
						default:
							{
								throw new ServiceCommunicationException(response.StatusCode);
							}
					}
				}
			}                
        }

        private async Task<TResultDataType> SendRequestV2Async<TResultDataType>(string query, HttpMethod method,
            CancellationToken cancellationToken = default)
        {
            return await SendRequestV2Async<object, TResultDataType>(query, method, null, cancellationToken: cancellationToken);
        }

        private async Task SendRequestV2Async(string query, HttpMethod method, object content, TimeSpan timeOut = default, CancellationToken cancellationToken = default)
        {
            await SendRequestV2Async<object, object>(query, method, content, timeOut, cancellationToken);
        }

        private async Task<object> SendRequestV2Async(string query, HttpMethod method, CancellationToken cancellationToken = default)
        {
            return await SendRequestV2Async<object>(query, method, cancellationToken);
        }

        #endregion

        #region Private Fields

        private readonly ConnectionStatusManager _connectionStatusManager;
        private readonly IConnectivity _connectivity;
        private const int TIME_OUT = 30;
        private DoctorServiceCommunicationToken _communicationToken;
		private readonly INativeHttpMessageHandlerProvider _nativeHttpMessageHandlerProvider;

		#endregion
	}
}