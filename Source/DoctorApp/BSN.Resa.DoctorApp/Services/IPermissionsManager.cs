using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Services
{
    /// <summary>
    /// This is the centralized place for app permissions.
    /// </summary>
    public interface IPermissionsManager
    {
        Task<bool> TryGetSmsPermissionAsync();

        Task<bool> IsPhonePermissionGrantedAsync();

        Task<bool> TryGetContactsPermissionAsync();

        void OpenResaAppSettingsPage();

        Task<bool> RequestAllRegularPermissionsAsync();

        bool CanDrawOverApps();

        Task<bool> AreAllPermissionsGrantedAsync();

        void OpenDrawOverAppsSettingPage();

        bool IsNotificationPolicyAccessGranted();

        void OpenNotificationPolicyAccessSettingPage();

        Task<bool> TryGetStoragePermissionAsync();

        void RedirectUserToPermissionsPage();
    }
}