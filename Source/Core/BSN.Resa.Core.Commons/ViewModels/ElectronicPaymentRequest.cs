using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class ElectronicPaymentRequest
    {
        public string Id { get; set; }

        public int Amount { get; set; }

        public string ReservationNumber { get; set; }

        public string CellphoneNumber { get; set; }

        public string InternalDescription { get; set; }

        public string GatewayDescription { get; set; }

        public string ServiceCallbackAddress { get; set; }

        public string ServiceCallbackMethod { get; set; }

        public string RedirectAddress { get; set; }

        public bool AutoVerify { get; set; }

        public DateTime BookedAt { get; set; }

        public DateTime? PaymentVerifiedAt { get; set; }

        public PaymentGatewayIdentifier? PaymentGatewayIdentifier { get; set; }

        public ElectronicPaymentRecord PaymentRecord { get; set; }
    }
}
