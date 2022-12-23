using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Prism.AppModel;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class UrgentUpdatePageViewModel : BindableBase, IPageLifecycleAware
    {
        #region Constructor

        public UrgentUpdatePageViewModel(
            IAppUpdateRepository appUpdateRepository,
            IAppUpdateHelper appUpdateHelper,
            IUserDialogs userDialogs,
            IApplicationManipulator applicationManipulator,
            IConnectivity connectivity,
            IPermissionsManager permissionsManager,
            IResaService resaService)
        {
            _userDialogs = userDialogs;
            _appUpdateHelper = appUpdateHelper;
            _appUpdateRepository = appUpdateRepository;
            _applicationManipulator = applicationManipulator;
            _connectivity = connectivity;
            _permissionsManager = permissionsManager;
            _resaService = resaService;
        }

        #endregion

        #region Life-cycle Methods

        public async void OnAppearing()
        {
            try
            {
                _resaService.Stop();

                _appUpdateHelper.CancelNotification();
            }
            catch
            {
                //Ignored
            }

            await ShowPopUpAsync();
        }

        public void OnDisappearing()
        {
            //Ignored
        }

        #endregion

        #region Bound Properties

        public ICommand PromptInstallUpdateCommand => new Command(async () => { await TryPromptInstallUpdate(); });

        public bool IsPromptInstallUpdateButtonVisible
        {
            get => _isPromptInstallUpdateButtonVisible;
            set => SetProperty(ref _isPromptInstallUpdateButtonVisible, value);
        }

        #endregion

        #region Private Methods

        private async Task ShowPopUpAsync()
        {
            if (IsDownloadFinished())
            {
                if (!await TryPromptInstallUpdate())
                    return;

                IsPromptInstallUpdateButtonVisible = true;

                return;
            }

            IsPromptInstallUpdateButtonVisible = false;

            if (await HasOngoingUpdateAsync())
            {
                CloseApplicationOnAvailability();

                return;
            }

            string updateDialogMessage = Resources.ResaAppNeedsUpdatePleaseDownloadMessage;

            if (_applicationManipulator.CanCloseApplicationGracefully)
            {
                string downloadApplicationUpdateDialogResult = await _userDialogs.ActionSheetAsync(
                    updateDialogMessage, Resources.Later, null, null, Resources.DownloadUpdate);

                if (downloadApplicationUpdateDialogResult == Resources.DownloadUpdate)
                {
                    if (await CheckStoragePermissionAsync())
                    {

                        await StartUpdateAsync();
                    }
                }
            }
            else
            {
                await _userDialogs.ActionSheetAsync(updateDialogMessage, Resources.DownloadUpdate, null, null);

                if (await CheckStoragePermissionAsync())
                {

                    await StartUpdateAsync();
                }
            }

            CloseApplicationOnAvailability();
        }

        private async Task<bool> TryPromptInstallUpdate()
        {
            if (!DownloadedFileExists())
            {
                await StartUpdateAsync();

                CloseApplicationOnAvailability();

                return false;
            }

            PromptInstallUpdate();

            return true;
        }

        private bool DownloadedFileExists()
        {
            return _appUpdateHelper.DoesDownloadedFileExist();
        }

        private void PromptInstallUpdate()
        {
            _appUpdateHelper.PromptUpdateInstall();
            DoctorAppSettings.IsUrgentUpdateDownloaded = false;

            CloseApplicationOnAvailability();
        }

        private async Task<bool> CheckStoragePermissionAsync()
        {
            return await _permissionsManager.TryGetStoragePermissionAsync();
        }

        private async Task<bool> HasOngoingUpdateAsync()
        {
            if (_appUpdateHelper.HasOngoingUpdate())
            {
                await _userDialogs.ActionSheetAsync(Resources.ResaIsBeingUpdatedInBg, Resources.Ok, null);

                return true;
            }

            return false;
        }

        private async Task StartUpdateAsync()
        {
            try
            {
                _appUpdateHelper.StartUpdate(_appUpdateRepository.Get(), _connectivity);
            }
            catch (ServiceCommunicationException)
            {
                string networkConnectionAttemptDialogResult = await _userDialogs.ActionSheetAsync(Resources.NoConnectionError, Resources.Later, null, null, Resources.Retry);

                if (networkConnectionAttemptDialogResult == Resources.Retry)
                    await StartUpdateAsync();
            }
        }

        private void CloseApplicationOnAvailability()
        {
            if (_applicationManipulator.CanCloseApplicationGracefully)
                _applicationManipulator.CloseApplicationGracefully();
        }

        private bool IsDownloadFinished()
        {
            return DoctorAppSettings.IsUrgentUpdateDownloaded;
        }

        #endregion

        #region Fields

        private readonly IUserDialogs _userDialogs;
        private readonly IAppUpdateHelper _appUpdateHelper;
        private readonly IAppUpdateRepository _appUpdateRepository;
        private readonly IApplicationManipulator _applicationManipulator;
        private readonly IConnectivity _connectivity;
        private readonly IPermissionsManager _permissionsManager;
        private readonly IResaService _resaService;
        private bool _isPromptInstallUpdateButtonVisible;

        #endregion
    }
}