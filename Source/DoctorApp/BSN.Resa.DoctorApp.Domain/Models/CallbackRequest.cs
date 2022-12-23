using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using System;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Domain.Models
{
    public class CallbackRequest
    {
        public static CallbackRequest AddServiceCommunicator(CallbackRequest callbackRequest,
            DoctorServiceCommunicationToken communicationToken, IDoctorServiceCommunicator serviceCommunicator)
        {
            if (callbackRequest == null)
                return null;

            callbackRequest.SetServiceCommunicationToken(communicationToken);
            callbackRequest.SetServiceCommunicator(serviceCommunicator);
            return callbackRequest;
        }

        #region Properties

        public string Id { get; set; }

        public int CommunicationAttemptsCount { get; set; }

        public DateTime ConsentGivenAt { get; set; }

        public string CallerFullName { get; set; }

        public string CallerSubscriberNumber { get; set; }

        public string ReceiverFullName { get; set; }

        public string ReceiverSubscriberNumber { get; set; }

        public bool ReturnCallHasBeenEstablished { get; set; }

        public bool IsExpired { get; set; }

        public bool IsCallTried { get; set; }

        public DateTime LastCallTriedAt { get; set; }

        public bool IsSeen { get; set; }

        public bool IsEstablishedCallNotified { get; set; }

        public string Message { get; set; }

        public long? Credit { get; set; }

        public bool IsCancelled { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// This method resets all properties that are defined only in local CallbackRequest model, i.e
        /// the ones not exist in server-side model.
        /// </summary>
        public void ResetAppSideLocalValues()
        {
            IsCallTried = false;
            LastCallTriedAt = default;
            IsSeen = false;
            IsEstablishedCallNotified = false;
        }

        public async Task BookAsync()
        {
            if (_serviceCommunicator == null || _communicationToken == null)
                throw new NullReferenceException("Before booking the callback request, " +
                                                 "you should call SetCommunicationToken() and SetServiceCommunicator()");

            _serviceCommunicator.CommunicationToken = _communicationToken;
            await _serviceCommunicator.BookCallbackRequestAsync(Id).ConfigureAwait(false);
        }

        private void SetServiceCommunicationToken(DoctorServiceCommunicationToken communicationToken)
        {
            _communicationToken = communicationToken;
        }

        private void SetServiceCommunicator(IDoctorServiceCommunicator serviceCommunicator)
        {
            _serviceCommunicator = serviceCommunicator;
        }

        #endregion

        #region Private fields

        private DoctorServiceCommunicationToken _communicationToken;
        private IDoctorServiceCommunicator _serviceCommunicator;

        #endregion
    }
}