using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators.ApplicationServiceCommunicator;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators.DoctorServiceCommunicator;
using BSN.Resa.DoctorApp.Droid.DeviceManipulators;
using BSN.Resa.DoctorApp.Droid.EventConsumers.AppUpdateNotificationClickedConsumers;
using BSN.Resa.DoctorApp.Droid.EventConsumers.CallStateChangedConsumers;
using BSN.Resa.DoctorApp.Droid.EventConsumers.InternetAccessedConsumers;
using BSN.Resa.DoctorApp.Droid.EventConsumers.PatientAuthenticationInquirerAnsweredConsumers;
using BSN.Resa.DoctorApp.Droid.Infrastructure;
using BSN.Resa.DoctorApp.Droid.Utilities;
using BSN.Resa.DoctorApp.EventConsumers.InternetAccessedConsumers;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using Plugin.Badge;
using Plugin.Badge.Abstractions;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.LocalNotifications;
using Plugin.LocalNotifications.Abstractions;
using Plugin.Permissions;

namespace BSN.Resa.DoctorApp.Droid.Services
{
    // ReSharper disable once InconsistentNaming
    public partial class ResaService
    {
        internal IAppUpdateNotificationClickedConsumer AppUpdateNotificationClickedConsumer
        {
            get
            {
                UpdateDatabaseAccessObjects();
                return new AppUpdateNotificationClickedConsumer(_appUpdateRepository, _appUpdateHelper, _connectivity);
            }
        }

        internal IPatientAuthenticationInquirerAnsweredConsumer PatientAuthenticationInquirerAnsweredConsumer
        {
            get
            {
                UpdateDatabaseAccessObjects();
                return new PatientAuthenticationInquirerAnsweredConsumer(_doctorRepository, _unitOfWork);
            }
        }

        internal ICallStateChangedConsumer PatientCallHandler
        {
            get
            {
                UpdateDatabaseAccessObjects();
                return new CallStateChangedConsumer(_doctorRepository, _incomingCallHelper, _mobileContactManager,
                    _unitOfWork, _permissionsManager);
            }
        }

        internal ICallStateChangedConsumer CallbackRequestsChecker
        {
            get
            {
                UpdateDatabaseAccessObjects();
                return new CallbackRequestsCallStateChangedConsumer(_doctorRepository, _callbackRequestRepository,
                    _unitOfWork, _appIconBadge, _localNotifications, _crashReporter);
            }
        }

        private IInternetAccessedConsumer AppUpdateChecker
        {
            get
            {
                UpdateDatabaseAccessObjects();
                return new AppUpdateChecker(_appUpdateRepository, _unitOfWork, _appUpdateHelper, _config);
            }
        }

        private IInternetAccessedConsumer PhoneNumbersSynchronizer
        {
            get
            {
                UpdateDatabaseAccessObjects();
                return new PhoneNumbersSynchronizer(_doctorRepository, _unitOfWork, _crashReporter);
            }
        }

        private void UpdateDatabaseAccessObjects()
        {
            _databaseFactory = new DatabaseFactory(_dbConnection);
            _doctorRepository = new DoctorRepository(_databaseFactory, _applicationServiceCommunicator,
                _doctorServiceCommunicator);
            _callbackRequestRepository =
                new CallbackRequestRepository(_databaseFactory, _doctorRepository, _doctorServiceCommunicator);
            _appUpdateRepository = new AppUpdateRepository(_databaseFactory, _applicationServiceCommunicator);
            _unitOfWork = new UnitOfWork(_databaseFactory);
        }

        private void RegisterTypes()
        {
            _config = ConfigAndroid.Instance;
            _dbConnection = new DbConnectionAndroid();
            _appUpdateHelper = new AppUpdateHelperAndroid();
            _incomingCallHelper = new IncomingCallHelperAndroid();
            _connectionStatusManager = new ConnectionStatusManager();
            _connectivity = CrossConnectivity.Current;
            _localNotifications = CrossLocalNotifications.Current;
            _appIconBadge = CrossBadge.Current;
            _crashReporter = new AppCenterCrashReporter();
            _mobileContactManager = new MobileContactManagerAndroid(_crashReporter);
            _permissionsManager = new PermissionsManager(PermissionsImplementation.Current, ResaPermissions.Instance, userDialogs:null);
            _nativeHttpMessageHandlerProvider = new HttpMessageHandlerProviderAndroid();

			SetDoctorServiceCommunicator();

            _applicationServiceCommunicator = new ApplicationServiceCommunicator(_connectionStatusManager, _connectivity, _config, _nativeHttpMessageHandlerProvider);
        }

        private void SetDoctorServiceCommunicator()
        {
            var internetCommunicator =
                new DoctorServiceCommunicatorViaInternet(_connectivity, _config, _connectionStatusManager, _nativeHttpMessageHandlerProvider);
            var smsCommunicator = new DoctorServiceCommunicatorViaInternet(_connectivity, _config, _connectionStatusManager, _nativeHttpMessageHandlerProvider);
            internetCommunicator.SetNextCommunicator(smsCommunicator);//Chain of responsibility pattern
            _doctorServiceCommunicator = internetCommunicator;
        }

        #region Private Fields

        private ConnectionStatusManager _connectionStatusManager;
        private IConfig _config;
        private IConnectivity _connectivity;
        private IApplicationServiceCommunicator _applicationServiceCommunicator;
        private IDoctorServiceCommunicator _doctorServiceCommunicator;
        private IIncomingCallHelper _incomingCallHelper;
        private IAppUpdateHelper _appUpdateHelper;
        private IMobileContactManager _mobileContactManager;
        private IUnitOfWork _unitOfWork;
        private IAppUpdateRepository _appUpdateRepository;
        private IDoctorRepository _doctorRepository;
        private ICallbackRequestRepository _callbackRequestRepository;
        private IDatabaseFactory _databaseFactory;
        private IDbConnection _dbConnection;
        private ILocalNotifications _localNotifications;
        private IBadge _appIconBadge;
        private IPermissionsManager _permissionsManager;
        private ICrashReporter _crashReporter;
		private INativeHttpMessageHandlerProvider _nativeHttpMessageHandlerProvider;

		#endregion
	}
}