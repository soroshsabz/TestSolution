using BSN.Resa.DoctorApp.EventConsumers;
using System;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.PatientAuthenticationInquirerAnsweredConsumers
{
	public enum PatientAuthenticationInquirerResult
	{
		Yes, No, Unknown
	}

	public class PatientAuthenticationInquirerAnsweredEventArgs: EventArgs
	{
		public PatientAuthenticationInquirerAnsweredEventArgs(
			string phoneNumber, PatientAuthenticationInquirerResult result)
		{
			PhoneNumber = phoneNumber;
			Result = result;
		}

		public string PhoneNumber { get; }

		public PatientAuthenticationInquirerResult Result { get; }
	}

	public interface IPatientAuthenticationInquirerAnsweredConsumer : IEventConsumer
	{
		Task OnAnswered(object sender, PatientAuthenticationInquirerAnsweredEventArgs e);
	}
}
