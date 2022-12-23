using BSN.Resa.DoctorApp.Commons.Services;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace BSN.Resa.DoctorApp.Data
{
    public static class DoctorAppSettings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        #region Setting Constants
        private const string AppCenterIdKey = "appcenter_id";
        private const string AppCenterUserIdKey = "appcenter_user_id";
        private const string FireBasePushNotificationTokenKey = "firebase_push_notification_token";
        private const string ResaServiceRunningModeKey = "resa_service_running_mode"; //valid values: "normal" and "foreground"
        private const string IsForegroundServiceNotificationClickedKey = "is_foreground_service_notification_clicked";
        private const string IsPermissionPagePassedKey = "is_permission_page_passed";
        private const string IsDoctorLoggedInKey = "is_doctor_logged_in";
        private const string IsHandlingDrawOverAppsPermissionPendingKey = "is_handling_draw_over_apps_permission_pending";
        private const string CallbackRequestPhoneNumberKey = "callback_request_phone_number";
        private const string IsUrgentUpdateDownloadedKey = "is_urgent_update_downloaded";
        private const string UrgentUpdateDownloadIdKey = "urgent_update_download_id";
        private const string IsUrgentUpdatePagePassedKey = "is_urgent_update_page_passed";
        private const string IsBlockerServiceEnabledByUserKey = "is_blocker_service_enabled_by_user";
        #endregion

        #region Settings

        public static string AppCenterId
        {
            get => AppSettings.GetValueOrDefault(AppCenterIdKey, string.Empty);
            set => AppSettings.AddOrUpdateValue(AppCenterIdKey, value);
        }

        public static string AppCenterUserId
        {
            get => AppSettings.GetValueOrDefault(AppCenterUserIdKey, "User not Logged in!");
            set => AppSettings.AddOrUpdateValue(AppCenterUserIdKey, value);
        }

        public static string FireBasePushNotificationToken
        {
            get => AppSettings.GetValueOrDefault(FireBasePushNotificationTokenKey, string.Empty);
            set => AppSettings.AddOrUpdateValue(FireBasePushNotificationTokenKey, value);
        }

        public static ResaServiceRunningMode ResaServiceRunningMode
        {
            get
            {
                string stringMode = AppSettings.GetValueOrDefault(ResaServiceRunningModeKey, string.Empty);

                bool parseResult = Enum.TryParse(stringMode, true, out ResaServiceRunningMode runningMode);

                return parseResult ? runningMode : ResaServiceRunningMode.Foreground;
            }
            set => AppSettings.AddOrUpdateValue(ResaServiceRunningModeKey, value.ToString().ToLowerInvariant());
        }

        public static bool IsForegroundServiceNotificationClicked
        {
            get => AppSettings.GetValueOrDefault(IsForegroundServiceNotificationClickedKey, false);
            set => AppSettings.AddOrUpdateValue(IsForegroundServiceNotificationClickedKey, value);
        }

        public static bool IsPermissionPagePassed
        {
            get => AppSettings.GetValueOrDefault(IsPermissionPagePassedKey, false);
            set => AppSettings.AddOrUpdateValue(IsPermissionPagePassedKey, value);
        }

        public static bool IsDoctorLoggedIn
        {
            get => AppSettings.GetValueOrDefault(IsDoctorLoggedInKey, false);
            set => AppSettings.AddOrUpdateValue(IsDoctorLoggedInKey, value);
        }

        public static bool IsHandlingDrawOverAppsPermissionPending
        {
            get => AppSettings.GetValueOrDefault(IsHandlingDrawOverAppsPermissionPendingKey, false);
            set => AppSettings.AddOrUpdateValue(IsHandlingDrawOverAppsPermissionPendingKey, value);
        }

        public static string CallbackRequestPhoneNumber
        {
            get => AppSettings.GetValueOrDefault(CallbackRequestPhoneNumberKey, string.Empty);
            set => AppSettings.AddOrUpdateValue(CallbackRequestPhoneNumberKey, value);
        }

        public static bool IsUrgentUpdateDownloaded
        {
            get => AppSettings.GetValueOrDefault(IsUrgentUpdateDownloadedKey, false);
            set => AppSettings.AddOrUpdateValue(IsUrgentUpdateDownloadedKey, value);
        }

        public static long UrgentUpdateDownloadId
        {
            get => AppSettings.GetValueOrDefault(UrgentUpdateDownloadIdKey, default(long));
            set => AppSettings.AddOrUpdateValue(UrgentUpdateDownloadIdKey, value);
        }

        public static bool IsUrgentUpdatePagePassed
        {
            get => AppSettings.GetValueOrDefault(IsUrgentUpdatePagePassedKey, false);
            set => AppSettings.AddOrUpdateValue(IsUrgentUpdatePagePassedKey, value);
        }

        public static bool IsBlockerServiceEnabledByUser
        {
            get => AppSettings.GetValueOrDefault(IsBlockerServiceEnabledByUserKey, false);
            set => AppSettings.AddOrUpdateValue(IsBlockerServiceEnabledByUserKey, value);
        }

        #endregion
    }
}