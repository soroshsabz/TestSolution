using RestSharp;
using System.Net;
using System.Threading.Tasks;

#pragma warning disable CS0162

namespace BSN.Resa.Commons
{
    public class TelegramBotMediator
    {
        public static Task<HttpStatusCode> SendTextMessageAsync(string botAddress, string textMessageReportPath, string chatId, string message)
        {
            if (Debug)
                return Task.FromResult(HttpStatusCode.OK);

            var client = new RestClient(botAddress);
            var request = new RestRequest(textMessageReportPath, Method.POST);
            request.AddParameter("chatId", chatId);
            request.AddParameter(new Parameter() { Type = ParameterType.RequestBody, ContentType = HttpMediaType.TextPlane, Value = message });

            var tcs = new TaskCompletionSource<HttpStatusCode>();
            client.ExecuteAsync<int>(request, response => tcs.SetResult(response.StatusCode));
            return tcs.Task;
        }


#if DEBUG
	    private const bool Debug = true;
#else
		private const bool Debug = false;
#endif

    }
}