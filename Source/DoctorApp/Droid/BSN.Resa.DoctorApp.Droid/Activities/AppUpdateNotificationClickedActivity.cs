using Android.App;
using Android.Content;
using Android.OS;
using BSN.Resa.DoctorApp.Droid.Services;

namespace BSN.Resa.DoctorApp.Droid.Activities
{
	[Activity]
	public class AppUpdateNotificationClickedActivity : Activity
	{
		public const int NotificationId = 14535;

		protected override void OnCreate(Bundle savedInstanceState)
		{
		    Title = Locale.Resources.DoctorAppDownloadResaNewVersion;

            base.OnCreate(savedInstanceState);

			if (!ResaService.IsRunning)
				StartService(new Intent(this, typeof(ResaService)));

			ResaService.Current.AppUpdateNotificationClickedConsumer.OnClicked();

			Finish();
		}
	}
}