using Android.App;
using Android.Content;
using BSN.Resa.DoctorApp.Droid.Services;

namespace BSN.Resa.DoctorApp.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
	public class BootReceiver: BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
		    var resaService = new ResaService();

		    if (resaService.CanStart())
		    {
		        resaService.Start();
		    }
        }
	}
}