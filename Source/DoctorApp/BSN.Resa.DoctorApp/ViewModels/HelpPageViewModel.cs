using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class HelpPageViewModel : BaseViewModel
    {
        #region Constructor

        public HelpPageViewModel(INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager
            ) :base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask, doctorRepository,
                unitOfWork, crashReporter, callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
        }

        #endregion

        protected override bool HasPageChangingDoctorStateFeature { get;} = true;
    }
}