using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
	public class Settlement
	{
		public int Id { get; set; }

        public SettlementMethod Method { get; set; }
        
        public long BeneficiaryCardMsisdn { get; set; }

        public long BeneficiaryCardVsin { get; set; }
        
        public string BeneficiaryAccountFirstName { get; set; }

        public string BeneficiaryAccountLastName { get; set; }

        public string BeneficiaryAccountInternationalBankAccountNumber { get; set; }

        public int LastTransactionId { get; set; }

        public int Amount { get; set; }

        public bool IsCarriedOut { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
	}
}
