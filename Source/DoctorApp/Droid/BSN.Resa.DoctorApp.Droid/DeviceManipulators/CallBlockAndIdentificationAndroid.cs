using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using System;

namespace BSN.Resa.DoctorApp.Droid.DeviceManipulators
{
    public class CallBlockAndIdentificationAndroid: ICallBlockAndIdentification
	{
		public bool IsBlockingEnabled => true;

		public bool IsIdentificationEnabled => false;

		public void RefreshBlockedPhoneNumbers()
		{
			throw new NotImplementedException();
		}
	}
}