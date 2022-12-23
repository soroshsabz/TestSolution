using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Services;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class DoctorStatePageViewModel : BaseViewModel
    {
        #region Properties

        public DelegateCommand UpdateCommand { get; }

        public string DoctorFullName
        {
            get => _doctorFullName;
            set => SetProperty(ref _doctorFullName, value);
        }

        public string DoctorVsin
        {
            get => _doctorVsin;
            set => SetProperty(ref _doctorVsin, value);
        }

        public bool IsUpdateButtonVisible
        {
            get => _isUpdateButtonVisible;
            set => SetProperty(ref _isUpdateButtonVisible, value);
        }

        public string DoctorState
        {
            get => _doctorState;
            set => SetProperty(ref _doctorState, value);
        }

        #endregion

        #region Constructors

        public DoctorStatePageViewModel(
            IPageDialogService pageDialogService,
            IDoctorRepository doctorRepository,
            IAppUpdateRepository appUpdateRepository,
            IUnitOfWork unitOfWork,
            IConnectivity connectivity,
            ISmsTask smsTask,
            IGsmConnection gsmConnection,
            ICrashReporter crashReporter,
            IAppUpdateHelper appUpdateHelper,
            IConfig config,
            INavigationService navigationService,
            ConnectionStatusManager connectionStatusManager,
            IPermissionsManager permissionsManager,
            ICallbackRequestRepository callbackRequestRepository)
            : base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask,
                doctorRepository, unitOfWork, crashReporter, callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            _pageDialogService = pageDialogService;
            _doctorRepository = doctorRepository;
            _appUpdateRepository = appUpdateRepository;
            _unitOfWork = unitOfWork;
            _appUpdateHelper = appUpdateHelper;
            _connectivity = connectivity;
            _config = config;

            SetDoctorStateView(_doctorRepository.Get());

            UpdateCommand = new DelegateCommand(UpdateApplication);
        }

        #endregion

        #region Static Members

        private static bool _isDoctorInformationSynchronized;

        #endregion

        #region Public Methods

        public async void UpdateApplication()
        {
            if (!_appUpdateHelper.HasOngoingUpdate())
            {
                while (true)
                {
                    try
                    {
                        _appUpdateHelper.StartUpdate(_appUpdateRepository.Get(), _connectivity);
                    }
                    catch (ServiceCommunicationException)
                    {
                        string serviceCommunicationAttemptDialogResult =
                            await _pageDialogService.DisplayActionSheetAsync(Resources.NoConnectionError,
                                Resources.Later, null, Resources.Retry);

                        if (serviceCommunicationAttemptDialogResult == Resources.Retry)
                            continue;
                    }

                    break;
                }
            }

            if (_appUpdateHelper.HasOngoingUpdate())
                IsUpdateButtonVisible = false;
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await FetchUpdateInformationFromServerAsync();
            await FetchDoctorInformationFromServerAsync();
        }

        #endregion

        #region Protected Stuff

        protected override bool HasPageChangingDoctorStateFeature { get; } = false;

        #endregion

        #region Private Methods

        protected override void SetDoctorStateView(Doctor doctor)
        {
            DoctorVsin = doctor.DoctorVsin;
            DoctorFullName = $"{doctor.FirstName} {doctor.LastName}";
            DoctorState = doctor.State.ToLocalizedString();
            DoctorStateImageUrl = $"Assets/doctor_state_{doctor.State.ToString().ToLower()}.png";
        }

        public async Task FetchDoctorInformationFromServerAsync()
        {
            if (_isDoctorInformationSynchronized)
                return;

            try
            {
                Doctor doctor = _doctorRepository.Get();

                await doctor.UpdateAsync();

                _doctorRepository.Update();
                _unitOfWork.Commit();

                SetDoctorStateView(doctor);

                _isDoctorInformationSynchronized = true;
            }
            catch (ServiceCommunicationException)
            {
                // ignored
            }
            catch (Exception exception)
            {
                CrashReporter.SendException(exception);
            }
        }

        private async Task FetchUpdateInformationFromServerAsync()
        {
            try
            {
                IsUpdateButtonVisible = !_appUpdateHelper.HasOngoingUpdate() &&
                                        await _appUpdateRepository.Get().HasNotifiableUpdateAsync(_config.Version);

                _appUpdateRepository.Update();
                _unitOfWork.Commit();
            }
            catch (NetworkConnectionException)
            {
                IsUpdateButtonVisible = false;
            }
            catch (ServiceCommunicationException)
            {
                // ignored
            }
        }

        #endregion

        #region private fields

        private bool _isUpdateButtonVisible;
        private string _doctorState;
        private string _doctorFullName;
        private string _doctorVsin;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppUpdateRepository _appUpdateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPageDialogService _pageDialogService;
        private readonly IAppUpdateHelper _appUpdateHelper;
        private readonly IConnectivity _connectivity;
        private readonly IConfig _config;

        #endregion
    }
}