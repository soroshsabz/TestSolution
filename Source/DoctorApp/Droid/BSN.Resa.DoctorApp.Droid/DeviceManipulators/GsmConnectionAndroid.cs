using Android.OS;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;

namespace BSN.Resa.DoctorApp.Droid.DeviceManipulators
{
    public class GsmConnectionAndroid : IGsmConnection
	{
		public bool IsConnected
		{
			get
			{
				if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1)
				{
					return Android.Provider.Settings.Global.GetInt
						(
							Android.App.Application.Context.ContentResolver,
							Android.Provider.Settings.Global.AirplaneModeOn,
							0
						) == 0;
				}
				else
				{
					return Android.Provider.Settings.System.GetInt
						(
							Android.App.Application.Context.ContentResolver,
							Android.Provider.Settings.Global.AirplaneModeOn,
							0
						) == 0;
				}
			}
		}
	}
}