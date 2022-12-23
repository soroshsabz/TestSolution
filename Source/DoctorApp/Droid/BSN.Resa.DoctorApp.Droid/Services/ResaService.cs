using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Telephony;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Droid.Activities;
using BSN.Resa.DoctorApp.Droid.Receivers;
using System;

namespace BSN.Resa.DoctorApp.Droid.Services
{
    [Service]
    public partial class ResaService : Service, IResaService
    {
        #region Static Members

        public static ResaService Current { get; private set; }

        public static bool IsRunning { get; set; }

        private static bool _initialize;

        #endregion

        #region Constructors

        static ResaService()
        {
            Initialize();
        }

        private ResaService(Service context)
        {
            if (!IsRunning && _initialize)
            {
                _context = context;

                _telephonyManager = (TelephonyManager)context.GetSystemService(TelephonyService);

                _audioManager = (AudioManager)context.GetSystemService(AudioService);

                _notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                _phoneStateListener = new IncomingCallListener.MyPhoneStateListener(context);

                RegisterTypes();

                _connectivity.ConnectivityChanged += (sender, e) =>
                {
                    if (e.IsConnected)
                    {
                        AppUpdateChecker.OnInternetAccessed();
                        PhoneNumbersSynchronizer.OnInternetAccessed();
                    }
                };
            }
        }

        public ResaService()
        {
        }

        #endregion

        #region Life-cycle Methods

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override void OnDestroy()
        {
            IsRunning = false;
            Current._telephonyManager.Listen(Current._phoneStateListener, PhoneStateListenerFlags.None);
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags,
            int startId)
        {
            if (!IsRunning)
            {
                var action = intent?.Action;

                if (action?.ToLowerInvariant() == ActionStartForegroundService)
                {
                    InitForegroundService();
                }

                _initialize = true;
                Current = new ResaService(this);
                IsRunning = true;
                Current._telephonyManager.Listen(Current._phoneStateListener, PhoneStateListenerFlags.CallState);
            }

            return StartCommandResult.Sticky;
        }

        #endregion

        #region IResaService Methods

        public bool Start()
        {
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(ResaService));

            var currentServiceRunningMode = DoctorAppSettings.ResaServiceRunningMode;

            if (currentServiceRunningMode == ResaServiceRunningMode.Foreground)
            {
                //visit: https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/services/foreground-services
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    intent.SetAction(ActionStartForegroundService);
                    context.StartForegroundService(intent);
                }
                else
                {
                    context.StartService(intent);
                }
            }
            else
            {
                context.StartService(intent);
            }

            return true;
        }

        public bool Stop()
        {
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(ResaService));
            bool result = context.StopService(intent);
            IsRunning = false;

            return result;
        }

        public bool CanStart()
        {
            if (IsRunning)
                return false;

            if (!DoctorAppSettings.IsUrgentUpdatePagePassed)
                return false;

            if (!DoctorAppSettings.IsBlockerServiceEnabledByUser)
                return false;

            return DoctorAppSettings.IsDoctorLoggedIn;
        }

        #endregion

        #region Private Methods

        private static void Initialize()
        {
            _initialize = false;
            IsRunning = false;
        }

        private void InitForegroundService()
        {
            var contentIntent = new Intent(this, typeof(SplashActivity));
            contentIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            contentIntent.PutExtra("foregroundServiceNotification", true);

            int requestCode = 50; // this is an arbitrary number.
            //requestCode is used to retrieve the same pending intent instance later on (for cancelling, etc).
            //Here we don't actually need it, so this number should not be important.
            //visit: https://stackoverflow.com/a/21526963/5941852

            var contentPendingIntent =
                PendingIntent.GetActivity(this, requestCode, contentIntent, PendingIntentFlags.UpdateCurrent);

            var notification = new NotificationCompat.Builder(this, MainActivity.GeneralChannelId)
                .SetContentText(Locale.Resources.DoctorAppPatientCallsBlockerService)
                .SetSmallIcon(Resource.Drawable.resa_icon_notification)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon))
                .SetOngoing(true)
                .SetShowWhen(false)
                .SetAutoCancel(false)
                .SetSound(null, 0)
                .SetContentIntent(contentPendingIntent)
                .Build();

            StartForeground(NotificationId, notification);
        }

        #endregion

        #region Fields

        private readonly TelephonyManager _telephonyManager;
        private readonly AudioManager _audioManager;
        private readonly NotificationManager _notificationManager;
        private readonly IncomingCallListener.MyPhoneStateListener _phoneStateListener;
        private readonly Service _context;
        private RingerMode _ringerMode;
        private InterruptionFilter _interruptionFilter;
        private const int NotificationId = 1005; // this is an arbitrary number.
        private const string ActionStartForegroundService = "startforegroundservice";

        #endregion
    }
}