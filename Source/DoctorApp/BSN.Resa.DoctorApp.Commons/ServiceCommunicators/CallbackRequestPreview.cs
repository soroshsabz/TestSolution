using System;

namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
    public class CallbackRequestPreview
    {
        public string Id { get; set; }

        public int CommunicationAttemptsCount { get; set; }

        public DateTime ConsentGivenAt { get; set; }

        public string CallerFullName { get; set; }

        public string CallerSubscriberNumber { get; set; }

        public string ReceiverFullName { get; set; }

        public string ReceiverSubscriberNumber { get; set; }

        public bool ReturnCallHasBeenEstablished { get; set; }

        public bool IsExpired { get; set; }

        public string Message { get; set; }

        public long? Credit { get; set; }

        public bool IsCancelled { get; set; }
    }
}