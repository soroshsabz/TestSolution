namespace BSN.Resa.Core.Commons.ViewModels
{
	public class ElectronicPaymentReceipt
	{
		public bool IsSuccessful { get; set; }

		public int Amount { get; set; }

		public string ErrorMessage { get; set; }

		public ElectronicPaymentReceipt(bool isSuccessful, int amount, string errorMessage = null)
		{
			IsSuccessful = isSuccessful;
			Amount = amount;
			ErrorMessage = errorMessage;
		}
	}
}
