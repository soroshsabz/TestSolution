using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Views.Utilities;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.DoctorApp.ViewModels.CallbackRequests
{
    public class CallbackRequestsHistoryPageViewModel : CallbackRequestsBaseViewModel
    {
        #region Constructor

        public CallbackRequestsHistoryPageViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            ICallbackRequestRepository callbackRequestRepository,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IPermissionsManager permissionsManager,
            ICrashReporter crashReporter,
            ConnectionStatusManager connectionStatusManager
        ) : base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask, callbackRequestRepository,
            doctorRepository, unitOfWork, permissionsManager, crashReporter,  connectionStatusManager)
        {
        }

        #endregion

        #region Life-cycle methods

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await GetLatestCallbackRequestsFromServerAsync(ServerCallbackRequestsFetchType.All);
        }

        #endregion

        #region Overridden Stuff

        protected override IEnumerable<CallbackRequestBindableObject> GetCallbackRequests()
        {
            var callbackRequests = CallbackRequestRepository.GetMany(callbackRequest => !callbackRequest.IsCancelled);
            return callbackRequests.ToCallbackRequestWrappers();
        }

        protected override IOrderedEnumerable<CallbackRequestBindableObject> OrderCallbackRequests(IEnumerable<CallbackRequestBindableObject> callbackRequests)
        {
            return callbackRequests.OrderByDescending(callViewModel => callViewModel.CallbackRequest.ConsentGivenAt);
        }

        protected override bool HasPageChangingDoctorStateFeature { get;} = true;

        #endregion
    }
}