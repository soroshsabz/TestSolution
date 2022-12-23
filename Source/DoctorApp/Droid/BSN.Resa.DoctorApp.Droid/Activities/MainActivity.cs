using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Droid.Utilities;
using BSN.Resa.DoctorApp.Utilities;
using PanCardView.Droid;
using Plugin.Messaging;
using Plugin.Permissions;
using System.Net;
using Xamarin.Forms.Platform.Android;

namespace BSN.Resa.DoctorApp.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        #region Public Static stuff

        public static FormsAppCompatActivity Instance { get; private set; }
        public const string CallbackRequestsChannelId = "Notification.CallbackRequestsChannel";
        public const string GeneralChannelId = "Notification.GeneralChannel";

        #endregion

        #region Life-cycle Methods
        
        protected override void OnCreate(Bundle bundle)
        {           
            Instance = this;

            // TODO: Remove Layout in code
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            DisablerAndroidMessageHandlerEmitter.Register();
            DisablerTrustProvider.Register();

            // visit this link: https://forums.xamarin.com/discussion/91782/trust-anchor-for-certification-path-not-found
            ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
 
            base.OnCreate(bundle);

            // visit this link: https://forums.xamarin.com/discussion/91782/trust-anchor-for-certification-path-not-found
            ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
 
            Xamarin.Essentials.Platform.Init(this, bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            UserDialogs.Init(this);

            //see: https://github.com/cjlotz/Xamarin.Plugins/blob/master/Messaging/Details.md#autodial-android
            CrossMessaging.Current.Settings().Phone.AutoDial = true;

            CardsViewRenderer.Preserve();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            HandleIfForegroundServiceNotificationClicked();

            LoadApplication(new App(new DependenciesRegistrationInitializerAndroid()));

            CreateNotificationChannel();

            RunResaService();
        }

        #endregion

        #region Public Methods

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion

        #region IPlatformInitializer method



        #endregion

        #region Private Methods

        private void HandleIfForegroundServiceNotificationClicked()
        {
            var foregroundServiceNotificationParam = Intent.GetBooleanExtra("foregroundServiceNotification", false);

            if (foregroundServiceNotificationParam)
            {
                DoctorAppSettings.IsForegroundServiceNotificationClicked = true;
            }
        }

        private void RunResaService()
        {
            var resaService = DependencyInjectionHelper.Resolve<IResaService>();

            if (resaService.CanStart())
            {
                resaService.Start();
            }
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var callbackRequestsChannel = new NotificationChannel(CallbackRequestsChannelId,
                Locale.Resources.DoctorAppCallbackRequestsNotificationChannelTitle, NotificationImportance.Default)
            {
                Description = Locale.Resources.DoctorAppCallbackRequestsNotificationChannelDescription
            };
            callbackRequestsChannel.SetShowBadge(true);


            var downloadsChannel = new NotificationChannel(AppUpdateHelperAndroid.DownloadsNotificationChannelId,
                Locale.Resources.DoctorAppDownloadsNotificationChannelTitle, NotificationImportance.High)
            {
                Description = Locale.Resources.DoctorAppDownloadsNotificationChannelDescription
            };
            downloadsChannel.SetShowBadge(false);

            var generalChannel = new NotificationChannel(GeneralChannelId,
                Locale.Resources.DoctorAppGeneralNotificationChannelTitle, NotificationImportance.Default)
            {
                Description = Locale.Resources.DoctorAppGeneralNotificationChannelTitle
            };
            generalChannel.SetShowBadge(false);
            generalChannel.EnableVibration(false);
            generalChannel.SetSound(null, null);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(callbackRequestsChannel);
            notificationManager.CreateNotificationChannel(downloadsChannel);
            notificationManager.CreateNotificationChannel(generalChannel);
        }

        #endregion
    }
}