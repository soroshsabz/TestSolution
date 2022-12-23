using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Services;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class AboutPageViewModel : BaseViewModel
    {
        #region Constructor

        public AboutPageViewModel(
            IConfig config,
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            ConnectionStatusManager connectionStatusManager,
            IPermissionsManager permissionsManager
        ) :
            base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask, doctorRepository,
                unitOfWork, crashReporter, callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            _config = config;

            SetAboutText();

            SetResaContactMediumLabelText();

            ApplicationVersion = _config.Version.ToString();
        }

        #endregion

        #region Overridden Stuff

        protected override bool HasPageChangingDoctorStateFeature { get; } = true;

        #endregion

        #region Bound Properties

        public string ApplicationVersion { get; }

        public string ResaContactMediumLabelText
        {
            get => _resaContactMediumLabelText;
            set => SetProperty(ref _resaContactMediumLabelText, value);
        }

        public ICommand ContactResaCommand => new Command(async () =>
        {
            await Launcher.OpenAsync(_config.AboutPageResaContactMediumUri);
        });

        public ICommand OnPrivacyPolicyTappedCommand => new Command(() =>
        {
            Launcher.OpenAsync(new Uri(_config.ResaPrivacyPolicyUrl));
        });

        public string AboutText
        {
            get => _aboutText;
            set => SetProperty(ref _aboutText, value);
        }

        #endregion

        #region Private Methods

        private void SetResaContactMediumLabelText()
        {
            ResaContactMediumLabelText = _config.AboutPageResaContactMedium;
        }

        private void SetAboutText()
        {
            AboutText = _config.AboutPageAboutDescription;
        }

        #endregion

        #region Private Fields

        private readonly IConfig _config;
        private string _resaContactMediumLabelText;
        private string _aboutText;

        #endregion
    }
}