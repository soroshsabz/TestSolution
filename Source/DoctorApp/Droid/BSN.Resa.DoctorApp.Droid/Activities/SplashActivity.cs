using Android.App;
using Android.Content;
using Android.OS;

namespace BSN.Resa.DoctorApp.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Splash", // Indicates the theme to use for this activity
		MainLauncher = true, // Set it as boot activity
		NoHistory = true)] // Doesn't place it in back stack
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
            base.OnCreate(bundle);

            var intent = new Intent(this, typeof(MainActivity));
		    if (Intent.Extras != null)
		    {
		        intent.PutExtras(Intent.Extras); //directing splash screen's extras to MainActivity's.
            }
			StartActivity(intent);
		}
    }
}