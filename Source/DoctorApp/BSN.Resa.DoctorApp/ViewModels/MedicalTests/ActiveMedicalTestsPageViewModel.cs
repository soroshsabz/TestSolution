using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.DoctorApp.Views.MedicalTests;
using BSN.Resa.DoctorApp.Views.Utilities;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BSN.Resa.DoctorApp.Services;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels.MedicalTests
{
    public class ActiveMedicalTestsPageViewModel : AbstractMedicalTestPageViewModel
    {
        #region Constructor

        public ActiveMedicalTestsPageViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IMedicalTestRepository medicalTestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager
            ) : base(navigationService, connectivity, gsmConnection,
            pageDialogService, smsTask, doctorRepository, medicalTestRepository, unitOfWork, crashReporter, callbackRequestRepository,
            permissionsManager, connectionStatusManager)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;
        }

        #endregion

        #region Life-cycle Methods

        public override void OnAppearing()
        {
            base.OnAppearing();

            LoadActiveMedicalTestsFromDb();

            SyncMedicalTestsWithServerInBackground();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            _cancellationTokenSource?.Cancel();
        }

        #endregion

        #region Protected Stuff

        protected override bool HasPageChangingDoctorStateFeature { get; } = true;

        #endregion

        #region Bound Properties

        public ObservableCollection<MedicalTest> MedicalTests
        {
            get => _medicalTests;
            set => SetProperty(ref _medicalTests, value);
        }

        public ICommand ViewCommand => new Command<MedicalTest>(async (medicalTest) => { await GoToMedicalTestPage(medicalTest); });

        public MedicalTest SelectedMedicalTest
        {
            get => _selectedMedicalTest;
            set
            {
                SetProperty(ref _selectedMedicalTest, value);

                if (value != null)
                    GoToMedicalTestPage(value).Wait();
            }
        }

        public bool IsActiveMedicalTestsEmpty
        {
            get => _isActiveMedicalTestsEmpty;
            set => SetProperty(ref _isActiveMedicalTestsEmpty, value);
        }

        #endregion

        #region Private Methods

        private async Task GoToMedicalTestPage(MedicalTest medicalTest)
        {
            var parameter = new NavigationParameters
            {
                {"SelectedMedicalTestId", medicalTest.Id}
            };

            await _navigationService.NavigateAsync($"{nameof(MedicalTestPage)}", parameter);

            SelectedMedicalTest = null;
        }

        private void LoadActiveMedicalTestsFromDb()
        {
            var medicalTestsFromDb = MedicalTestRepository.GetAll(true)?.ToList();

            if (medicalTestsFromDb == null || !medicalTestsFromDb.Any())
            {
                IsActiveMedicalTestsEmpty = true;

                MedicalTests = null;
            }
            else
            {
                IsActiveMedicalTestsEmpty = false;

                Device.InvokeOnMainThreadAsync(() => { MedicalTests = medicalTestsFromDb.OrderBy(medicalTest => medicalTest.UpdatedAt).ToObservableCollection(); });
            }
        }

        private void SyncMedicalTestsWithServerInBackground()
        {
            if (!_connectivity.IsConnected)
                return;

            InitCancellationToken();

            Task.Run(async () =>
            {
                await SyncDbMedicalTestsWithServer(_taskCancellationToken);

                LoadActiveMedicalTestsFromDb();

            }, _taskCancellationToken);
        }

        private void InitCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _taskCancellationToken = _cancellationTokenSource.Token;
        }

        #endregion

        #region Private Fields

        private readonly INavigationService _navigationService;
        private ObservableCollection<MedicalTest> _medicalTests;
        private MedicalTest _selectedMedicalTest;
        private bool _isActiveMedicalTestsEmpty;
        private readonly IConnectivity _connectivity;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _taskCancellationToken;

        #endregion
    }
}