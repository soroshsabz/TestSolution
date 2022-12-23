namespace BSN.Resa.DoctorApp.Droid.Utilities
{
	public interface IIncomingCallHelper
	{
		void DisconnectCall();

		void MuteRinging();

		void ShowPatientAuthenticationInquirerDialog(string phoneNumber);
	}
}
