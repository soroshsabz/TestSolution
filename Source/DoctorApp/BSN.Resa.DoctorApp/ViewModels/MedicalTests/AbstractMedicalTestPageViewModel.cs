using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Services;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.ViewModels.MedicalTests
{
    public abstract class AbstractMedicalTestPageViewModel : BaseViewModel
    {
        #region Constructor

        protected AbstractMedicalTestPageViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IMedicalTestRepository medicalTestRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager) : base(navigationService, connectivity, gsmConnection,
            pageDialogService, smsTask, doctorRepository, unitOfWork, crashReporter, callbackRequestRepository,
            permissionsManager, connectionStatusManager)
        {
            _doctorRepository = doctorRepository;
            MedicalTestRepository = medicalTestRepository;
        }

        #endregion

        #region Protected Stuff

        protected override bool HasPageChangingDoctorStateFeature { get; } = true;

        protected readonly IMedicalTestRepository MedicalTestRepository;

        protected async Task SyncDbMedicalTestsWithServer(CancellationToken cancellationToken = default)
        {
            try
            {
                var doctor = _doctorRepository.Get(includeContacts: false);

                var medicalTests = await doctor.GetAllActiveMedicalTests(cancellationToken);

                if (medicalTests == null)
                    return;

                MedicalTestRepository.DeleteAll();

                UnitOfWork.Commit();

                if (medicalTests.Any())
                {
                    MedicalTestRepository.AddRange(medicalTests);

                    UnitOfWork.Commit();
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region Private Fields

        private readonly IDoctorRepository _doctorRepository;

        #endregion
    }
}