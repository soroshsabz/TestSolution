using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Views;
using BSN.Resa.DoctorApp.Views.CallbackRequests;
using BSN.Resa.Locale;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using FlyoutPage = BSN.Resa.DoctorApp.Views.FlyoutPage;

namespace BSN.Resa.DoctorApp.ViewModels
{
    /// <summary>
    /// This is the main place to get all necessary Resa application permissions from user.
    /// Other places that we get permissions are per case and in the same place that
    /// the related and associated feature is used.
    /// Note: There are two scenarios that user is directed ti this page, one is immediately after
    /// login and before going to main page, and the second scenario is in ResaService, where whenever
    /// there is an incoming call and before Resa call blocker does its job(showing dialog and blocking call),
    /// we check if the required permissions for Resa call blocker are not granted, we direct user to this page.
    /// </summary>
    public class PermissionsPageViewModel : BindableBase, IPageLifecycleAware
    {
        #region Constructor

        public PermissionsPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogs,
            IDeviceInfo deviceInfo,
            IResaService resaService,
            IPermissionsManager permissionsManager
        )
        {
            _navigationService = navigationService;
            _userDialogs = userDialogs;
            _deviceInfo = deviceInfo;
            _resaService = resaService;
            _permissionsManager = permissionsManager;
        }

        #endregion

        #region Life-cycle Stuff

        public async void OnAppearing()
        {
            if (await _permissionsManager.AreAllPermissionsGrantedAsync())
            {
                await GoToHomePageAsync();
            }
            else
            {
                IsMainContentVisible = true;
            }
        }

        public void OnDisappearing()
        {
            //Ignored
        }

        #endregion

        #region Bound properties

        public bool IsMainContentVisible
        {
            get => _isMainContentVisible;
            set => SetProperty(ref _isMainContentVisible, value);
        }

        public bool IsOpenAppSettingsPageButtonVisible
        {
            get => _isOpenAppSettingsPageButtonVisible;
            set => SetProperty(ref _isOpenAppSettingsPageButtonVisible, value);
        }

        public ICommand GoToHomeCommand => new Command(async () =>
        {
            bool areAllRegularPermissionsGranted = await _permissionsManager.RequestAllRegularPermissionsAsync();

            if (!areAllRegularPermissionsGranted)
            {
                await _userDialogs.AlertAsync(Resources.DoctorAppPermissionsGeneralRationaleAndHowToEnable,
                    okText: Resources.Ok);

                IsOpenAppSettingsPageButtonVisible = true;
            }
            else if (!CanDrawOverApps())
            {
                await _userDialogs.AlertAsync(
                    Resources.DoctorAppDrawOverAppPermissionPleaseEnable, okText: Resources.Ok);

                _permissionsManager.OpenDrawOverAppsSettingPage();
            }
            else if (!IsNotificationPolicyAccessGranted())
            {
                await _userDialogs.AlertAsync(Resources.DoctorAppPermissionsNotificationRationale,
                    okText: Resources.Ok);

                OpenNotificationPolicyAccessSettingPage();
            }
            else if (await _permissionsManager.AreAllPermissionsGrantedAsync())
            {
                if (IsItAMiuiDevice())
                {
                    await _userDialogs.AlertAsync(Resources.DoctorAppPermissionsAutostartRationale,
                        okText: Resources.Ok);

                    OpenAutostartSettingsPageCommand();
                }

                await GoToHomePageAsync();
            }
        });

        public ICommand OpenAppSettingsPageCommand =>
            new Command(() => { _permissionsManager.OpenResaAppSettingsPage(); });

        #endregion

        #region Private Methods

        private bool IsItAMiuiDevice()
        {
            return Device.RuntimePlatform == Device.Android && _deviceInfo.IsDeviceXiaomiMiui();
        }

        private async Task GoToHomePageAsync()
        {
            DoctorAppSettings.IsPermissionPagePassed = true;

            DoctorAppSettings.IsBlockerServiceEnabledByUser = true;

            DoctorAppSettings.ResaServiceRunningMode = ResaServiceRunningMode.Foreground;

            _resaService.Start();

            await _navigationService.NavigateAsync(
                $"/{nameof(FlyoutPage)}/{nameof(AppNavigationPage)}/{nameof(CallbackRequestsPage)}");
        }

        private bool CanDrawOverApps()
        {
            return Device.RuntimePlatform == Device.iOS || _permissionsManager.CanDrawOverApps();
        }

        private void OpenAutostartSettingsPageCommand()
        {
            _deviceInfo.OpenXiaomiMiuiAutostartSettingsPage();
        }

        private bool IsNotificationPolicyAccessGranted()
        {
            return Device.RuntimePlatform == Device.iOS || _permissionsManager.IsNotificationPolicyAccessGranted();
        }

        private void OpenNotificationPolicyAccessSettingPage()
        {
            _permissionsManager.OpenNotificationPolicyAccessSettingPage();
        }

        #endregion

        #region Private Fileds

        private readonly INavigationService _navigationService;
        private readonly IUserDialogs _userDialogs;
        private readonly IDeviceInfo _deviceInfo;
        private readonly IResaService _resaService;
        private readonly IPermissionsManager _permissionsManager;
        private bool _isMainContentVisible;
        private bool _isOpenAppSettingsPageButtonVisible;

        #endregion
    }
}