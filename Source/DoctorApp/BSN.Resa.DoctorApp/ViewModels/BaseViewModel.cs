using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Aspects;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.DoctorApp.Views;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using FlyoutPage = BSN.Resa.DoctorApp.Views.FlyoutPage;

namespace BSN.Resa.DoctorApp.ViewModels
{
    /// <summary>
    /// this class is meant to be used as the base and top level viewmodel, so that shared and reusable codes can be
    /// used among all other viewmodels.
    /// </summary>
    public abstract class BaseViewModel : BindableBase, IPageLifecycleAware, IApplicationLifecycleAware, INavigatedAware
    {
        #region Constructor

        protected BaseViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager)
        {
            _pageDialogService = pageDialogService;
            _connectivity = connectivity;
            _doctorRepository = doctorRepository;
            UnitOfWork = unitOfWork;
            _callbackRequestRepository = callbackRequestRepository;
            PermissionsManager = permissionsManager;
            _navigationService = navigationService;
            _gsmConnection = gsmConnection;
            _smsTask = smsTask;
            CrashReporter = crashReporter;

            _cancellationTokenSource = new CancellationTokenSource();
            _taskCancellationToken = _cancellationTokenSource.Token;

            ChangeDoctorStateCommand = new DelegateCommand(async () => await ChangeDoctorState(), () => CanChangeDoctorState);

            SetDoctorStateView(_doctorRepository.Get());

            InitConnectionStatusChangedStuff(connectionStatusManager);
        }

        #endregion

        #region Life cycle methods

        public virtual async void OnAppearing()
        {
            UpdateUnSeenCallbackRequestsCount();

            if (HasPageChangingDoctorStateFeature)
            {
                //we do this because server returns doctor's state back to "Available" after a specific time
                await GetDoctorStateFromServerAsync(_taskCancellationToken);
            }
        }

        public virtual void OnDisappearing()
        {
            _cancellationTokenSource?.Cancel();

            UnRegisterConnectionStatusChangedEvents();

            UnRegisterOnSendDoctorStateResultListener();
        }

        public virtual void OnResume()
        {
            //Ignored
        }

        public virtual void OnSleep()
        {
            //Ignored
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            _cancellationTokenSource?.Cancel();
            UnRegisterConnectionStatusChangedEvents();
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            //Ignored here
        }

        #endregion

        #region Public Properties

        public bool IsConnectingToServer
        {
            get => _isConnectingToServer || _showConnectingToServer;
            set => SetProperty(ref _isConnectingToServer, value);
        }

        public ICommand ShowNavDrawerCommand
        {
            get
            {
                return new Command(() =>
                {
                    App.CustomFlyoutPage.IsPresented = !App.CustomFlyoutPage.IsPresented;
                });
            }
        }

        public bool IsCallbackCountVisible
        {
            get => _isCallbackCountVisible;
            set
            {
                _isCallbackCountVisible = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource DoctorStateImageUrl
        {
            get => _doctorStateImageUrl;
            set => SetProperty(ref _doctorStateImageUrl, value);
        }

        public bool CanChangeDoctorState
        {
            get => _canChangeDoctorState;
            set => SetProperty(ref _canChangeDoctorState, value);
        }

        public DelegateCommand ChangeDoctorStateCommand { get; }

        public int UnSeenCallbackCount
        {
            get => _unSeenCallbackCount;
            set
            {
                SetProperty(ref _unSeenCallbackCount, value);
                IsCallbackCountVisible = value >= 1;
            }
        }

        public ICommand ResaBottomBarTabSelectedCommand => new Command(async (pageName) =>
        {
            await _navigationService.NavigateAsync(
                $"/{nameof(FlyoutPage)}/{nameof(AppNavigationPage)}/{pageName}");
        });

        #endregion

        #region Protected Methods

        protected async Task ShowInternetNotAvailableAlertAsync()
        {
            await _pageDialogService.DisplayAlertAsync(null, Resources.NoConnectionError, Resources.Ok);
        }

        protected async Task ShowAppInternalErrorAlertAsync()
        {
            await _pageDialogService.DisplayAlertAsync(null, Resources.InternalError, Resources.Close);
        }

        protected virtual void SetDoctorStateView(Doctor doctor)
        {
            var state = doctor.State;

            if (state == Commons.DoctorState.Available)
                DoctorStateImageUrl = "Assets/doctor_state_available_toolbar.png";
            else if (state == Commons.DoctorState.Office)
                DoctorStateImageUrl = "Assets/doctor_state_office_toolbar.png";
            else
                DoctorStateImageUrl = $"Assets/doctor_state_{state.ToString().ToLower()}.png";
        }

        protected void UpdateUnSeenCallbackRequestsCount()
        {
            UnSeenCallbackCount =
                _callbackRequestRepository
                    .UnSeenCount(); //TODO: do sth so that this be called only once not in all page's OnAppearing
        }

        #endregion

        #region Protected Fields

        protected readonly ICrashReporter CrashReporter;
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IPermissionsManager PermissionsManager;

        #endregion

        #region Abstract Stuff

        protected abstract bool HasPageChangingDoctorStateFeature { get; }

        #endregion

        #region Static Stuff

        private static DateTime _lastTimeGotDoctorStateFromServer;

        #endregion

        #region Private Methods

        /// <summary>
        /// Since in some regular intervals doctor's state changes by
        /// server(for example server changes state to Available if it's been remaining to not
        /// available for 4 hours. I'm not sure about number), therefore whenever
        /// MainPage resumes we get the latest state from server.
        /// </summary>
        /// <returns></returns>
        private async Task GetDoctorStateFromServerAsync(CancellationToken cancellationToken)
        {
            if (!_connectivity.IsConnected)
                return; //check internet. GetDoctorState isn't implemented via SMS mechanism so we don't need to check GSM connection

            var now = DateTime.Now;

            //has the last time that we got doctor's state been past 20 minutes or more?(for overload purposes)
            bool minimumTimePassed = _lastTimeGotDoctorStateFromServer.AddMinutes(20) <= now;

            bool canRequest = _lastTimeGotDoctorStateFromServer == default || minimumTimePassed;

            if (!canRequest)
                return;

            try
            {
                Doctor doctor = _doctorRepository.Get();

                if (doctor == null)
                    return;

                var oldState = doctor.State;

                var newState = await doctor.GetStateAsync(cancellationToken);
                _lastTimeGotDoctorStateFromServer = DateTime.Now;

                if (oldState == newState)
                    return; //doctor's state not changed so no need to update DB and UI

                _doctorRepository.Update();
                UnitOfWork.Commit();

                SetDoctorStateView(doctor);
            }
            catch (OperationCanceledException)
            {
                //Ignored as this is not unwanted
            }
            catch (Exception exception)
            {
                CrashReporter.SendException(exception);
            }
        }

        private async Task ChangeDoctorState()
        {
            // This checking is for fast responding
            if (!(_connectivity.IsConnected || (_gsmConnection.IsConnected && _smsTask.CanSendSmsInBackground)))
            {
                await _pageDialogService.DisplayAlertAsync(Resources.ConnectionError,
                    Resources.NoGsmOrInternetError,
                    Resources.Close);
                return;
            }

            // Ensuring SMS permission is already granted, otherwise requesting it from user,
            // because if internet is not available we change doctor status via sms and doing so requires SMS permission.
            // For more info see DoctorServiceCommunicatorViaSms class.
            if (!await PermissionsManager.TryGetSmsPermissionAsync())
                return;

            string selectedText = await _pageDialogService.DisplayActionSheetAsync(Resources.SelectYourState,
                Resources.Close,
                null,
                typeof(Commons.DoctorState).ValuesAsLocalizedStrings().ToArray());

            Commons.DoctorState? selectedState = selectedText.To<Commons.DoctorState>();
            if (selectedState == null)
                return;

            await ChangeDoctorState(selectedState.Value);
        }

        [CentralizedExceptionHandler]
        private async Task ChangeDoctorState(Commons.DoctorState selectedState)
        {
            Doctor doctor = _doctorRepository.Get();

            if (selectedState == doctor.State)
                return;

            CanChangeDoctorState = false;

            try
            {
                doctor.OnSendDoctorStateResult += DoctorOnSendDoctorStateResult;

                await doctor.SetStateAsync(selectedState);
            }
            catch (NoGsmConnectionException)
            {
                await _pageDialogService.DisplayAlertAsync(Resources.ConnectionError,
                    Resources.NoGsmOrInternetError,
                    Resources.Close);
            }
            catch (SmsSendingException)
            {
                await _pageDialogService.DisplayAlertAsync(Resources.ConnectionError,
                    Resources.SmsSendError,
                    Resources.Close);
            }
            catch (TimeoutException)
            {
                string selectedAction = await _pageDialogService.DisplayActionSheetAsync(Resources.NoServiceError,
                    null,
                    null,
                    Resources.Retry,
                    Resources.Cancel);

                if (selectedAction == Resources.Retry)
                    await ChangeDoctorState(selectedState);
            }
            catch (AuthenticationException)
            {
                //Ignored here, because ConnectionStatusManager will handle this exception by logging out.
            }
            catch (Exception)
            {
                CanChangeDoctorState = true;

                throw;
            }
            finally
            {
                CanChangeDoctorState = true;
            }
        }

        private async void DoctorOnSendDoctorStateResult(Commons.DoctorState state, bool isSuccessful)
        {
            UnRegisterOnSendDoctorStateResultListener();

            if (!isSuccessful)
            {
                await ShowChangeStatusFailedMessage();

                return;
            }

            _doctorRepository.Update();
            UnitOfWork.Commit();

            SetDoctorStateView(_doctorRepository.Get());
        }

        private void UnRegisterOnSendDoctorStateResultListener()
        {
            Doctor doctor = _doctorRepository.Get();

            doctor.OnSendDoctorStateResult -= DoctorOnSendDoctorStateResult;
        }

        private async Task ShowChangeStatusFailedMessage()
        {
            await _pageDialogService.DisplayAlertAsync("", Resources.DoctorAppChangingDoctorStatusFailedMessage, Resources.Ok);
        }

        #region ConnectionStatusChangedStuff

        private void InitConnectionStatusChangedStuff(ConnectionStatusManager connectionStatusManager)
        {
            _connectionStatusManager = connectionStatusManager;
            _connectionStatusManager.ConnectionStatusChanged += ConnectionStatusChangedHandler;
            _connectionStatusManager.OnAuthenticationStatusChanged += AuthenticationStatusChangedHandler;
            _showConnectingToServer = false;
            IsConnectingToServer = false;
        }

        private void UnRegisterConnectionStatusChangedEvents()
        {
            _connectionStatusManager.ConnectionStatusChanged -= ConnectionStatusChangedHandler;
            _connectionStatusManager.OnAuthenticationStatusChanged -= AuthenticationStatusChangedHandler;
        }

        private void AuthenticationStatusChangedHandler(AuthenticationStatus status)
        {
            var fileManager = DependencyInjectionHelper.Resolve<IDbConnection>();
            fileManager.DeleteDatabaseFile();

            var resaService = DependencyInjectionHelper.Resolve<IResaService>();
            resaService.Stop();

            DoctorAppSettings.IsBlockerServiceEnabledByUser = false;

            DoctorAppSettings.IsDoctorLoggedIn = false;

            _navigationService.NavigateAsync($"/{nameof(LoginPage)}");
        }

        public void ConnectionStatusChangedHandler(ConnectionStatus connectionStatus)
        {
            if (connectionStatus == ConnectionStatus.Started)
                IsConnectingToServer = true;
            else
                IsConnectingToServer = false;
        }

        protected void ShowConnectingToServer()
        {
            _showConnectingToServer = true;
            RaisePropertyChanged(nameof(IsConnectingToServer));
        }

        protected void HideConnectingToServer()
        {
            _showConnectingToServer = false;
            _isConnectingToServer = false;
            RaisePropertyChanged(nameof(IsConnectingToServer));
        }

        #endregion

        #endregion

        #region Fields

        private int _unSeenCallbackCount;
        private bool _isCallbackCountVisible;
        private readonly IConnectivity _connectivity;
        private readonly IGsmConnection _gsmConnection;
        private readonly ISmsTask _smsTask;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ICallbackRequestRepository _callbackRequestRepository;
        private ImageSource _doctorStateImageUrl;
        private bool _canChangeDoctorState = true;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _taskCancellationToken;
        private readonly IPageDialogService _pageDialogService;
        private bool _showConnectingToServer;
        private ConnectionStatusManager _connectionStatusManager;
        private bool _isConnectingToServer;
        private readonly INavigationService _navigationService;

        #endregion
    }
}