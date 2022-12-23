using Android;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Droid.Activities;
using BSN.Resa.DoctorApp.Services;
using Java.Lang;
using Plugin.Permissions;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using AndroidPermission = Android.Content.PM.Permission; //AndroidPermission as an alias for Android's native Permission type
using Application = Android.App.Application;
using CrossPermission = Plugin.Permissions.Abstractions.Permission;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

//CrossPermission as an alias for CrossPermission' Permission type

namespace BSN.Resa.DoctorApp.Droid.Utilities
{
    public class ResaPermissions : IResaPermissions
    {
        #region Static Stuff

        private static IResaPermissions _instance;

        public static IResaPermissions Instance
        {
            get => _instance ?? (_instance = new ResaPermissions());
            set => _instance = value;
        }

        #endregion

        #region Constructor

        public ResaPermissions()
        {
            _context = Application.Context;
        }

        #endregion

        #region IResaPermissions Methods

        public bool CanDrawOverApps()
        {
            var result = Build.VERSION.SdkInt < BuildVersionCodes.M || Settings.CanDrawOverlays(_context);
            return result;
            //return Build.VERSION.SdkInt >= BuildVersionCodes.M ? Settings.CanDrawOverApps(Application.Context) : true;
        }

        public void OpenDrawOverAppsSettingPage()
        {
            Intent myIntent = new Intent(Settings.ActionManageOverlayPermission,
                Uri.Parse("package:" + _context.PackageName));
            myIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.NoHistory);
            _context.StartActivity(myIntent);
        }

        /// <summary>
        /// we request this particular permission because the permission library that we use (i.e PermissionsPlugin)
        /// does NOT support ANSWER_PHONE_CALLS permission yet.
        /// Visit: https://github.com/jamesmontemagno/PermissionsPlugin/issues/156
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RequestAnswerPhoneCallsPermissionAsync()
        {
            try
            {
                if (MainActivity.Instance == null)
                    return false;

                ActivityCompat.RequestPermissions(MainActivity.Instance, new[] { Manifest.Permission.AnswerPhoneCalls },
                    2);
                await Task.Delay(500);
                return IsAnswerPhoneCallsPermissionGranted();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsNotificationPolicyAccessGranted()
        {
            NotificationManager notificationManager =
                (NotificationManager)_context.GetSystemService(Context.NotificationService);
            bool isGranted = Build.VERSION.SdkInt < BuildVersionCodes.M ||
                             notificationManager.IsNotificationPolicyAccessGranted;
            return isGranted;
        }

        public void OpenNotificationPolicyAccessSettingPage()
        {
            Intent intent;

            if (IsItAnLgG4Device())
            {
                //for more info why the setting for an LG G4 is different visit:
                //https://stackoverflow.com/a/42663282/5941852
                intent = new Intent(Settings.ActionNotificationListenerSettings);
            }
            else
            {
                intent = new Intent(Settings.ActionNotificationPolicyAccessSettings);
            }

            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.NoHistory);
            _context.StartActivity(intent);
        }

        /// <summary>
        /// Check if all required permissions are already grated. This will be used for example by
        /// ResaService before doing any task requiring permission.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AreAllEssentialPermissionsGrantedAsync()
        {
            bool areRegularPermissionsGranted = true;

            CrossPermission[] permissions = {
                CrossPermission.Contacts, CrossPermission.Phone,
                CrossPermission.Sms, CrossPermission.Storage
            };

            foreach (CrossPermission permission in permissions)
            {
                var thePermissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

                if (thePermissionStatus != PermissionStatus.Granted)
                {
                    areRegularPermissionsGranted = false;
                    break;
                }
            }

            bool areAllPermissionsGranted =
                areRegularPermissionsGranted && IsAnswerPhoneCallsPermissionGranted() && CanDrawOverApps();

            return areAllPermissionsGranted;
        }

        /// <summary>
        /// This method is ued in IncomingCallListener class, and whenever there is an incoming call we check if required
        /// permissions are already granted, otherwise we use this method to direct user to permissions page in app.
        /// </summary>
        public void RedirectUserToPermissionsPage()
        {
            DoctorAppSettings.IsPermissionPagePassed = false;

            Intent intent = new Intent(Application.Context, typeof(SplashActivity));
            intent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }

        public bool IsAnswerPhoneCallsPermissionGranted()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return true; //Because this permission was introduced in API 26.
            //Visit: https://developer.android.com/reference/android/Manifest.permission.html#ANSWER_PHONE_CALLS

            var result = ContextCompat.CheckSelfPermission(_context, Manifest.Permission.AnswerPhoneCalls);
            return result == AndroidPermission.Granted;
        }

        #endregion

        #region Private Methods

        private bool IsItAnLgG4Device()
        {
            string[] manufacturerVariants = { "lg", "lge" };

            //models gathered from following sources:
            //https://en.wikipedia.org/wiki/LG_G4
            //https://www.devicespecifications.com/en/model/52f833d1
            string[] models = { "lg-h810", "lg h810", "h810", "lg-h811", "lg h811", "h811", "lg-h815", "lg h815", "h815", "lg-h815t", "lg h815t", "h815t", "lg-h818", "lg h818", "h818", "lg-h818n", "lg h818n", "h818n", "lg-h819", "lg h819", "h819", "lg-vs986", "lg vs986", "vs986", "lg-ls991", "lg ls991", "ls991", "lg-us991", "lg us991", "us991", "lg-f500k", "lg f500k", "f500k", "lg-f500l", "lg f500l", "f500l", "lg-f500s", "lg f500s", "f500s" };

            string deviceManufacturer = DeviceInfo.Manufacturer.ToLowerInvariant();
            string deviceModel = DeviceInfo.Model.ToLowerInvariant();

            bool isManufacturerLg = manufacturerVariants.Any(manufacturer => manufacturer.Equals(deviceManufacturer));
            bool isModelG4 = models.Any(model => model.Equals(deviceModel));

            return isManufacturerLg && isModelG4;
        }

        #endregion

        #region Private Fields

        private readonly Context _context;

        #endregion
    }
}