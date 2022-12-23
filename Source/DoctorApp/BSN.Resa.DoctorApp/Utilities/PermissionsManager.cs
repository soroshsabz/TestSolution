using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.Locale;
using Plugin.Permissions.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace BSN.Resa.DoctorApp.Utilities
{
    /// <summary>
    /// This is the centralized place for app permissions.
    /// </summary>
    public class PermissionsManager : IPermissionsManager
    {
        #region Constructor

        public PermissionsManager(
            IPermissions pluginPermissions,
            IResaPermissions resaPermissions,
            IUserDialogs userDialogs)
        {
            _pluginPermissions = pluginPermissions;
            _resaPermissions = resaPermissions;
            _userDialogs = userDialogs;
            _isDeviceIos = DeviceInfo.Platform == DevicePlatform.iOS;
        }

        #endregion

        /// <summary>
        /// Returns true if SMS permission is granted, otherwise alerts user with a dialog
        /// saying internet is not available and then returns false.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryGetSmsPermissionAsync()
        {
            var result = await _pluginPermissions.RequestPermissionsAsync(Permission.Sms);
            bool isSmsPermissionGranted = result[Permission.Sms] == PermissionStatus.Granted;

            if (!isSmsPermissionGranted)
            {
                await ShowInternetNotAvailableAlertAsync();

                return false;
            }

            return true;
        }

        public async Task<bool> IsPhonePermissionGrantedAsync()
        {
            var permissionRequestResult = await _pluginPermissions.RequestPermissionsAsync(Permission.Phone);
            bool isCallPermissionGranted = permissionRequestResult[Permission.Phone] == PermissionStatus.Granted;
            return isCallPermissionGranted;
        }

        public async Task<bool> TryGetContactsPermissionAsync()
        {
            bool isPermissionGranted = await RequestPermissionAsync(Permission.Contacts);

            if (!isPermissionGranted)
            {
                var accepted = await ShowContactsPermissionNeededAlertAsync();

                if (!accepted)
                    return false;

                isPermissionGranted = await RequestPermissionAsync(Permission.Contacts);

                return isPermissionGranted;
            }

            return true;
        }

        public void OpenResaAppSettingsPage()
        {
            _pluginPermissions.OpenAppSettings();
        }

        public async Task<bool> RequestAllRegularPermissionsAsync()
        {
            var result = await _pluginPermissions.RequestPermissionsAsync(RegularPermissions);
            bool isAnswerPhoneCallsPermissionGranted = await RequestAnswerPhoneCallsPermissionAsync();

            return result.All(permissionResult => permissionResult.Value == PermissionStatus.Granted)
                   && isAnswerPhoneCallsPermissionGranted;
        }

        public bool CanDrawOverApps()
        {
            return _isDeviceIos || _resaPermissions.CanDrawOverApps();
        }

        public async Task<bool> AreAllPermissionsGrantedAsync()
        {
            bool areRegularPermissionsGranted = true;

            foreach (Permission permission in RegularPermissions)
            {
                var thePermissionStatus = await _pluginPermissions.CheckPermissionStatusAsync(permission);
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

        public void OpenDrawOverAppsSettingPage()
        {
            if (_isDeviceIos)
                return;

            _resaPermissions.OpenDrawOverAppsSettingPage();
        }

        public bool IsNotificationPolicyAccessGranted()
        {
            return _isDeviceIos || _resaPermissions.IsNotificationPolicyAccessGranted();
        }

        public void OpenNotificationPolicyAccessSettingPage()
        {
            if (_isDeviceIos)
                return;

            _resaPermissions.OpenNotificationPolicyAccessSettingPage();
        }

        public async Task<bool> TryGetStoragePermissionAsync()
        {
            var statuses = await _pluginPermissions.RequestPermissionsAsync(Permission.Storage);

            if (statuses[Permission.Storage] != PermissionStatus.Granted)
            {
                var userChoice = await ShowStoragePermissionNeededAlertAsync();

                if (!userChoice)
                    return false;

                var statusesSecondTry = await _pluginPermissions.RequestPermissionsAsync(Permission.Storage);

                bool isGrantedTheSecondTime = statusesSecondTry[Permission.Storage] == PermissionStatus.Granted;

                return isGrantedTheSecondTime;
            }

            return true;
        }

        public void RedirectUserToPermissionsPage()
        {
            if (_isDeviceIos)
                return;

            _resaPermissions.RedirectUserToPermissionsPage();
        }

        #region Private Methods

        private async Task<bool> RequestPermissionAsync(Permission permission)
        {
            var status = await _pluginPermissions.RequestPermissionsAsync(permission);
            bool isPermissionGranted = status[permission] == PermissionStatus.Granted;
            return isPermissionGranted;
        }

        private Permission[] RegularPermissions => new[]
        {
            Permission.Contacts, Permission.Phone, Permission.Sms, Permission.Storage
        };

        private async Task<bool> ShowContactsPermissionNeededAlertAsync()
        {
            var result = await _userDialogs.ConfirmAsync(Resources.DoctorAppContactsPermissionRationalleDescription,
                okText: Resources.Ok, cancelText: Resources.Cancel);

            return result;
        }

        private async Task ShowInternetNotAvailableAlertAsync()
        {
            await _userDialogs.AlertAsync(Resources.NoConnectionError, okText: Resources.Ok);
        }

        private bool IsAnswerPhoneCallsPermissionGranted()
        {
            return _isDeviceIos || _resaPermissions.IsAnswerPhoneCallsPermissionGranted();
        }

        private async Task<bool> RequestAnswerPhoneCallsPermissionAsync()
        {
            if (_isDeviceIos)
                return true;

            return await _resaPermissions.RequestAnswerPhoneCallsPermissionAsync();
        }

        private async Task<bool> ShowStoragePermissionNeededAlertAsync()
        {
            var result = await _userDialogs.ConfirmAsync(
                Resources.DoctorAppStoragePermissionRationalleDescription, okText: Resources.Ok,
                cancelText: Resources.Cancel);
            return result;
        }

        private readonly bool _isDeviceIos;
        #endregion

        #region Private Fields

        private readonly IPermissions _pluginPermissions;
        private readonly IResaPermissions _resaPermissions;
        private readonly IUserDialogs _userDialogs;

        #endregion
    }
}