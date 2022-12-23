#pragma warning disable CS0162

using System.Net;
using System.Threading.Tasks;

namespace BSN.Resa.Commons
{
    public interface ISmsSender
    {
        Task<HttpStatusCode> SendSmsAsync(string message, string recipient);
        Task<HttpStatusCode> SendVerificationCodeAsync(string verificationCode, string recipient);
        Task<HttpStatusCode> SendLookupAsync(string recipient, string template, string token, string token2 = null, string token3 = null, string token10 = null, string token20 = null);
    }
}