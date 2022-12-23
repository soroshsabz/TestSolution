using BSN.Resa.DoctorApp.EventConsumers;
using System;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.CallStateChangedConsumers
{
	public enum CallState
	{
		Started, Ended
	}

	public class CallStateChangedEventArges: EventArgs
	{
		public CallStateChangedEventArges(CallState state, string phoneNumber)
		{
			State = state;
			PhoneNumber = phoneNumber;
		}

		public CallState State { get; }

		public string PhoneNumber { get; }
	}

	public interface ICallStateChangedConsumer : IEventConsumer
	{
		void OnCallStateChanged(object sender, CallStateChangedEventArges e);
	}
}
