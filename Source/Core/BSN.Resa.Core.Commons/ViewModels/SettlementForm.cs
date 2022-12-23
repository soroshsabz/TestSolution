using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.ViewModels
{
	public class SettlementForm
	{
        [EnumDataType(typeof(SettlementMethod))]
        public SettlementMethod Method { get; set; }

        [Required]
		public long? BeneficiaryCardSerialNumber { get; set; }

		[Required]
		public int? LastTransactionId { get; set; }
        
		public string Description { get; set; }

		public SettableStatus State { get; set; }

	}
}