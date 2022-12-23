using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Services
{
    public interface IResaPermissions
    {
        bool CanDrawOverApps();

        void OpenDrawOverAppsSettingPage();

        /// <summary>
        /// we check this particular permission because the permission library that we use (i.e PermissionsPlugin)
        /// does NOT support ANSWER_PHONE_CALLS permission yet.
        /// Visit: https://github.com/jamesmontemagno/PermissionsPlugin/issues/156
        /// </summary>
        /// <returns></returns>
        Task<bool> RequestAnswerPhoneCallsPermissionAsync();

        /// <summary>
        /// Check if ACCESS_NOTIFICATION_POLICY permission of Android is granted.
        /// Note: in Android versions >= 7, this permission must be granted to be able to mute incoming calls ring tone,
        /// Other wise "Java.Lang.SecurityException: Not allowed to change Do Not Disturb state" will be throw-ed.
        /// Note: We check this particular permission because the permission library that we use (i.e PermissionsPlugin)
        /// does NOT support ANSWER_PHONE_CALLS permission yet.
        /// Visit: https://github.com/jamesmontemagno/PermissionsPlugin/issues/156
        /// </summary>
        /// <returns></returns>
        bool IsNotificationPolicyAccessGranted();

        void OpenNotificationPolicyAccessSettingPage();

        Task<bool> AreAllEssentialPermissionsGrantedAsync();

        void RedirectUserToPermissionsPage();

        bool IsAnswerPhoneCallsPermissionGranted();
    }
}
