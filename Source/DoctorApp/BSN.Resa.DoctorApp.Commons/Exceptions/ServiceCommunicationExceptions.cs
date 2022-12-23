using System;
using System.Net;

namespace BSN.Resa.DoctorApp.Commons.Exceptions
{
    public class ServiceCommunicationException : Exception
    {
        public ServiceCommunicationException()
        {
        }

        public ServiceCommunicationException(string message) : base(message)
        {
        }

        public ServiceCommunicationException(string message, Exception innerException) : base(message)
        {
        }

        public ServiceCommunicationException(HttpStatusCode httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
    }

    public class NetworkConnectionException : ServiceCommunicationException
    {
        public NetworkConnectionException()
        {
        }

        public NetworkConnectionException(bool isOperationCancelled)
        {
            IsOperationCancelled = isOperationCancelled;
        }

        public bool IsOperationCancelled { get; set; }
    }

    public class InternetNotAvailableException : NetworkConnectionException { }

    public class NoGsmConnectionException : ServiceCommunicationException { }

    public class SmsSendingException : ServiceCommunicationException { }

    public class PaymentRequiredException : ServiceCommunicationException { }

    public class AuthenticationException : ServiceCommunicationException { }

    public class ResaJsonException : ServiceCommunicationException
    {
        public ResaJsonException()
        {
        }

        public ResaJsonException(string message) : base(message)
        {
        }
    }
}
