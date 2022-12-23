using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable CS0162

namespace BSN.Resa.Commons
{
    public class KaveNegarSmsSender : ISmsSender
    {
        private const string KavenegarApiKey = "4C4E566B45446D2B30395373764279614259714153413D3D";
        private const string KavenegarLookupTemplate = "verify";

        public Task<HttpStatusCode> SendSmsAsync(string message, string recipient)
        {
            if (Debug)
                return Task.FromResult(HttpStatusCode.OK);

            var client = new RestClient("https://api.kavenegar.com");
            var request = new RestRequest("v1/{key}/sms/send.json");
            request.AddUrlSegment("key", KavenegarApiKey);
            request.AddParameter("receptor", recipient);
            request.AddParameter("message", message);

            var tcs = new TaskCompletionSource<HttpStatusCode>();
            client.ExecuteAsync<int>(request, response => tcs.SetResult(response.StatusCode));
            return tcs.Task;
        }

        public Task<HttpStatusCode> SendVerificationCodeAsync(string verificationCode, string recipient)
        {
            if (Debug)
                return Task.FromResult(HttpStatusCode.OK);

            var client = new RestClient("https://api.kavenegar.com");
            var request = new RestRequest("v1/{key}/verify/lookup.json");
            request.AddUrlSegment("key", KavenegarApiKey);
            request.AddParameter("receptor", recipient);
            request.AddParameter("token", verificationCode);
            request.AddParameter("template", KavenegarLookupTemplate);

            var tcs = new TaskCompletionSource<HttpStatusCode>();
            client.ExecuteAsync<int>(request, response => tcs.SetResult(response.StatusCode));
            return tcs.Task;
        }



        public Task<HttpStatusCode> SendLookupAsync(string recipient, string template, string token,
            string token2 = null, string token3 = null, string token10 = null, string token20 = null)
        {
            if (Debug)
                return Task.FromResult(HttpStatusCode.OK);

            var client = new RestClient("https://api.kavenegar.com");
            var request = new RestRequest("v1/{key}/verify/lookup.json");
            request.AddUrlSegment("key", KavenegarApiKey);
            request.AddParameter("receptor", recipient);
            request.AddParameter("token", SuitableToken(token));
            request.AddParameter("template", template);

            if (!string.IsNullOrEmpty(token2))
                request.AddParameter("token2", SuitableToken(token2));

            if (!string.IsNullOrEmpty(token3))
                request.AddParameter("token3", SuitableToken(token3));

            if (!string.IsNullOrEmpty(token10))
                request.AddParameter("token10", SuitableToken(token10));

            if (!string.IsNullOrEmpty(token20))
                request.AddParameter("token20", SuitableToken(token20));

            var tcs = new TaskCompletionSource<HttpStatusCode>();
            client.ExecuteAsync<int>(request, response => tcs.SetResult(response.StatusCode));
            return tcs.Task;
        }

        private string SuitableToken(string input)
        {
            string semiSpace = "-";

            if (string.IsNullOrWhiteSpace(input))
                input = semiSpace;

            return input.Trim().Replace(" ", semiSpace);
        }

#if DEBUG
        private const bool Debug = true;
#else
		private const bool Debug = false;
#endif

    }
}