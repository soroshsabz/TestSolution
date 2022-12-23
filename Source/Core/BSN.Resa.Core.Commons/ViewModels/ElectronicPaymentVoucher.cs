using System.Collections.Generic;

namespace BSN.Resa.Core.Commons.ViewModels
{
	public class ElectronicPaymentVoucher
	{
        #region Properties

        public string PaymentRequestId { get; set; }

        public int Amount { get; set; }

		public string GatewayAddress { get; set; }

        public ElectronicPaymentSubmissionType SubmissionType { get; set; }

        public Dictionary<string, string> SubmissionParameters { get; set; }

        #endregion

        public ElectronicPaymentVoucher(string paymentRequestId, int amount, string gatewayAddress,
            ElectronicPaymentSubmissionType submissionType = ElectronicPaymentSubmissionType.Anchor,
            Dictionary<string, string> submissionParameters = null)
		{
			PaymentRequestId = paymentRequestId;
            Amount = amount;
			GatewayAddress = gatewayAddress;
            SubmissionType = submissionType;
            SubmissionParameters = submissionParameters ?? new Dictionary<string, string>();
		}
	}

    public enum ElectronicPaymentSubmissionType
    {
        Anchor,
        Form
    }
}
