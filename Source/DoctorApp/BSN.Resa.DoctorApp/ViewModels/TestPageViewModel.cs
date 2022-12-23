using System;
using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Utilities;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using BSN.Resa.DoctorApp.Aspects;
using BSN.Resa.DoctorApp.Services;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class TestPageViewModel : BaseViewModel
    {
        #region Constructor

        public TestPageViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IUserDialogs userDialogs,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager,
            IMedicalTestRepository medicalTestRepository
        ) : base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask, doctorRepository,
            unitOfWork, crashReporter, callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            _callbackRequestRepository = callbackRequestRepository;
            _medicalTestRepository = medicalTestRepository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _navigationService = navigationService;
            _userDialogs = userDialogs;
        }

        #endregion

        protected override bool HasPageChangingDoctorStateFeature { get;} = true;

        public ICommand ActionCommand => new Command(async () => { await Test(); });

        public ICommand CancelCommand => new Command(() =>
        {
            
        });

        private async Task Test()
        {
            try
            {
                var doctor = _doctorRepository.Get();
                var result = await doctor.GetAllActiveMedicalTests();
                _medicalTestRepository.AddOrUpdateRange(result);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string EntryValue
        {
            get => _entryValue;
            set => SetProperty(ref _entryValue, value);
        }

        #region Private Fields

        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICallbackRequestRepository _callbackRequestRepository;
        private readonly IMedicalTestRepository _medicalTestRepository;
        private readonly INavigationService _navigationService;
        private readonly IUserDialogs _userDialogs;
        private string _entryValue;

        #endregion
    }
}