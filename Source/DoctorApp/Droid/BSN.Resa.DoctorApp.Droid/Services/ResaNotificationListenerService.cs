using Android;
using Android.App;
using Android.Service.Notification;

namespace BSN.Resa.DoctorApp.Droid.Services
{
    //visit these links to see why we used this class:
    //https://stackoverflow.com/a/42663282/5941852
    //https://forums.xamarin.com/discussion/166297/how-to-check-for-incoming-notifications-in-xamarin-android
    [Service(Label = "Resa Notification Service", Permission = Manifest.Permission.BindNotificationListenerService)]
    [IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
    public class ResaNotificationListenerService : NotificationListenerService
    {
    }
}