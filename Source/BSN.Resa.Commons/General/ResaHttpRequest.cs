using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using BSN.Resa.Commons.General;
using Newtonsoft.Json;

namespace BSN.Resa.Commons
{
    public class HttpMediaType
    {
        public const string TextPlane = MediaTypeNames.Text.Plain;
        public const string ApplicationJson = "application/json";
    }

    public class ResaHttpRequest
    {
        private readonly string _to;

        private ResaHttpRequest(string to)
        {
            _to = to;
        }

        public static ResaHttpRequest To(string to)
        {
            return new ResaHttpRequest(to);
        }

        public HttpClient Connect()
        {
            var client = new HttpClient { BaseAddress = new Uri(_to) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpMediaType.ApplicationJson));
            return client;
        }

        #region Get

        public Task<HttpResponseMessage> GetAsync(string requestUri, string query = "", HttpContent content = null)
        {
            return SendRequestAsync(requestUri, HttpMethod.Get, query, content);
        }

        #endregion

        #region Delete

        [Obsolete("To be replaced with asynchronous approach")]
        public HttpResponseMessage Delete(string requestUri, string query = "")
        {
            return DeleteAsync(requestUri, query).ResultWithUnwrappedExceptions();
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, string query = "")
        {
            HttpClient client = Connect();
            HttpResponseMessage response = await client.DeleteAsync(requestUri + "?" + query).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new ConnectionException(response, "Connection or operation on " + requestUri + " failed.");
            return response;
        }

        #endregion

        #region Post

        [Obsolete("To be replaced with asynchronous approach")]
        public HttpResponseMessage PostAsJson<T>(string requestUri, T content, string query = "", string mediaType = HttpMediaType.ApplicationJson)
        {
            return PostAsJsonAsync(requestUri, content, query, mediaType).ResultWithUnwrappedExceptions();
        }

        public Task<HttpResponseMessage> PostAsJsonAsync(string requestUri, string query = "")
        {
            return PostAsJsonAsync(requestUri, new StringContent(string.Empty), query);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T content, string query = "", string mediaType = HttpMediaType.ApplicationJson, JsonSerializerSettings jsonSerializerSettings = null)
        {
            HttpClient client = Connect();
            HttpResponseMessage response;

            if (mediaType == HttpMediaType.TextPlane && content is string)
            {
                response = await client.PostAsync($"{requestUri}?{query}", new StringContent(content as string, Encoding.UTF8, HttpMediaType.TextPlane))
                    .ConfigureAwait(false);
            }
            else if (mediaType == HttpMediaType.ApplicationJson)
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(content, jsonSerializerSettings);
                response = await client.PostAsync($"{requestUri}?{query}", new StringContent(jsonData, Encoding.UTF8, HttpMediaType.ApplicationJson))
                    .ConfigureAwait(false);
            }
            else
            {
                throw new UnsupportedMediaTypeException($"Unsupported media type {mediaType}", new MediaTypeHeaderValue(mediaType));
            }

            if (!response.IsSuccessStatusCode)
                throw new ConnectionException(response, "Connection or operation on " + response.RequestMessage.RequestUri + " failed.");

            return response;
        }

        [Obsolete("To be replaced with asynchronous approach")]

        public HttpResponseMessage PostAsJson(string requestUri, string query = "")
        {
            return PostAsJsonAsync(requestUri, new StringContent(string.Empty), query).ResultWithUnwrappedExceptions();
        }

        #endregion

        #region Put

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T content, string query = "")
        {
            HttpClient client = Connect();
            HttpResponseMessage response = await client.PutAsync($"{requestUri}?{query}", new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(content), Encoding.UTF8, HttpMediaType.ApplicationJson))
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new ConnectionException(response, "Connection or operation on " + requestUri + " failed.");

            return response;
        }

        [Obsolete("To be replaced with asynchronous approach")]
        public HttpResponseMessage PutAsJson<T>(string requestUri, T content, string query = "")
        {
            return PutAsJsonAsync(requestUri, content, query).ResultWithUnwrappedExceptions();
        }

        [Obsolete("To be replaced with asynchronous approach")]
        public HttpResponseMessage PutAsJson(string requestUri, string query = "")
        {
            return PutAsJson(requestUri, new StringContent(""), query);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync(string requestUri, string query = "")
        {
            return PutAsJsonAsync(requestUri, new StringContent(""), query);
        }

        #endregion

        #region Patch

        public async Task<HttpResponseMessage> PatchAsJsonAsync<T>(string requestUri, T content, string query = "")
        {
            var serializer = new JavaScriptSerializer() { MaxJsonLength = 5000000 };

            HttpClient client = Connect();
            HttpResponseMessage response = await client.PatchAsync($"{requestUri}?{query}", new StringContent(serializer.Serialize(content), Encoding.UTF8, HttpMediaType.ApplicationJson))
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new ConnectionException(response, "Connection or operation on " + requestUri + " failed.");

            return response;
        }

        public Task<HttpResponseMessage> PatchAsJsonAsync(string requestUri, string query = "")
        {
            return PatchAsJsonAsync(requestUri, new StringContent(string.Empty), query);
        }

        #endregion

        #region Generic Request

        [Obsolete("To be replaced with asynchronous approach")]
        public HttpResponseMessage SendRequest(string requestUri, HttpMethod method, string query = "", HttpContent content = null)
        {
            return SendRequestAsync(requestUri, method, query, content).ResultWithUnwrappedExceptions();
        }

        public async Task<HttpResponseMessage> SendRequestAsync(string requestUri, HttpMethod method, string query = "", HttpContent content = null)
        {
            HttpClient client = Connect();
            var message = new HttpRequestMessage(method, requestUri + "?" + query)
            {
                Content = content
            };
            HttpResponseMessage response = await client.SendAsync(message).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new ConnectionException(response, "Connection or operation on " + response.RequestMessage.RequestUri + " failed.");
            }
            return response;
        }


        #endregion
    }
}