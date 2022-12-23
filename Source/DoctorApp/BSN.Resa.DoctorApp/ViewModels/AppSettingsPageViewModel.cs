using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.DoctorApp.Views;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;
using BSN.Resa.DoctorApp.Services;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class AppSettingsPageViewModel : BaseViewModel
    {
        public AppSettingsPageViewModel(INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager) :
            base(navigationService, connectivity, gsmConnection,
                pageDialogService, smsTask, doctorRepository, unitOfWork, crashReporter,
                callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            _navigationService = navigationService;
        }

        protected override bool HasPageChangingDoctorStateFeature { get;} = true;

        public ICommand OnAdvancedSettingsTappedCommand => new Command(arg =>
        {
            _navigationService.NavigateAsync(nameof(AppSettingsAdvancedPage));
        });

        private readonly INavigationService _navigationService;
    }
}