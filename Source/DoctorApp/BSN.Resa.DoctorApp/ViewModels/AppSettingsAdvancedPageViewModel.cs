using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class AppSettingsAdvancedPageViewModel : BaseViewModel
    {
        #region Constructor

        public AppSettingsAdvancedPageViewModel(INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IResaService resaService,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager) :
            base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask, doctorRepository,
                unitOfWork, crashReporter, callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            _resaService = resaService;
            _pageDialogService = pageDialogService;
            Initialize();
        }

        #endregion

        #region Bound Properties

        public bool IsBlockerServiceToggled
        {
            get => _isBlockerServiceToggled;
            set
            {
                if (_isBlockerServiceToggled != value)
                {
                    _isBlockerServiceToggled = value;

                    RaisePropertyChanged();

                    UpdateBlockerServiceState(value);
                }
            }
        }

        public bool IsForegroundModeToggled
        {
            get => _isForegroundModeToggled;
            set
            {
                if (_isForegroundModeToggled != value)
                {
                    _isForegroundModeToggled = value;

                    RaisePropertyChanged();

                    UpdateServiceRunningMode(value);
                }
            }
        }

        public ICommand ShowForegroundServiceInfoCommand => new Command(() =>
        {
            _pageDialogService.DisplayAlertAsync("", Resources.DoctorAppAppSettingsImproveBgBlockerServiceDescription,
                Resources.Ok);
        });

        #endregion

        #region Protected Stuff

        protected override bool HasPageChangingDoctorStateFeature { get; } = true;

        #endregion

        #region Private Methods

        private void UpdateBlockerServiceState(bool isEnable)
        {
            DoctorAppSettings.IsBlockerServiceEnabledByUser = isEnable;

            IsForegroundModeToggled = isEnable;

            if (!isEnable)
            {
                _resaService.Stop();
            }
        }

        private void UpdateServiceRunningMode(bool isForeground)
        {
            DoctorAppSettings.ResaServiceRunningMode = isForeground ? ResaServiceRunningMode.Foreground : ResaServiceRunningMode.Normal;

            if (!IsBlockerServiceToggled)
                return;

            _resaService.Stop();

            _resaService.Start();
        }

        private void Initialize()
        {
            _isBlockerServiceToggled = DoctorAppSettings.IsBlockerServiceEnabledByUser;

            var currentMode = DoctorAppSettings.ResaServiceRunningMode;

            if (_isBlockerServiceToggled)
            {
                _isForegroundModeToggled = currentMode == ResaServiceRunningMode.Foreground;
            }
            else
            {
                _isForegroundModeToggled = false;
            }
        }

        #endregion

        #region Private Fields

        private bool _isForegroundModeToggled;
        private readonly IResaService _resaService;
        private readonly IPageDialogService _pageDialogService;
        private bool _isBlockerServiceToggled;

        #endregion
    }
}