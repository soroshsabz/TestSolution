using Android.App;
using Android.OS;
using BSN.Resa.DoctorApp.Droid.Activities;
using BSN.Resa.DoctorApp.Utilities;
using Plugin.CurrentActivity;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Droid.Utilities
{
    public class ApplicationManipulatorAndroid: IApplicationManipulator
	{
		public bool CanCloseApplicationGracefully => true;

		public void CloseApplicationGracefully()
		{
			CloseApplication();
		}

		/// <summary>
		/// Notice that it just closes current main activity and
		/// doesn't stop current running thread, 
		/// so the execution of current thread continued.
		/// Using System.Threading.Thread.CurrentThread.Abort() just sometimes
		/// works. 
		/// </summary>
		public void CloseApplication()
		{
			// TODO: How to close application correctly?
			MainActivity.Instance.FinishAffinity();

			// https://stackoverflow.com/questions/47353986/xamarin-forms-forms-context-is-obsolete
			CrossCurrentActivity.Current.Activity.FinishAffinity();
		}
	}
}