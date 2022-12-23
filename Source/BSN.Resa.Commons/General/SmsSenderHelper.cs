using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BSN.Resa.Commons
{
    public static class SmsSenderHelper
    {
        public static Task<HttpStatusCode> SendLookupAsync(string recipient, string template, string token, string token2 = null, string token3 = null, string token10 = null, string token20 = null)
        {
            return _smsSender.SendLookupAsync(recipient, template, token, token2, token3, token10, token20);
        }

        public static Task<HttpStatusCode> SendSmsAsync(string message, string recipient)
        {
            return _smsSender.SendSmsAsync(message, recipient);
        }

        public static Task<HttpStatusCode> SendVerificationCodeAsync(string verificationCode, string recipient)
        {
            return _smsSender.SendVerificationCodeAsync(verificationCode, recipient);
        }

        private readonly static ISmsSender _smsSender = new KaveNegarSmsSender();
    }
}
