using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Domain.Utilities;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels.CallbackRequests
{
    public abstract class CallbackRequestsBaseViewModel : BaseViewModel
    {
        #region Public Static Methods

        internal static CallbackRequestGroupList.CallbackRequestGroup CalculateCallbackRequestGroup(CallbackRequestBindableObject callbackRequestBindableObject)
        {
            if (callbackRequestBindableObject?.CallbackRequest == null || callbackRequestBindableObject.CallbackRequest.ConsentGivenAt == default(DateTime))
                return CallbackRequestGroupList.CallbackRequestGroup.Undefined;

            var dateTime = callbackRequestBindableObject.CallbackRequest?.ConsentGivenAt;

            if (dateTime?.Date == DateTime.Today)
            {
                return CallbackRequestGroupList.CallbackRequestGroup.Today;
            }

            if (dateTime?.Date == DateTime.Today.Subtract(TimeSpan.FromDays(1)).Date)
            {
                return CallbackRequestGroupList.CallbackRequestGroup.Yesterday;
            }

            return CallbackRequestGroupList.CallbackRequestGroup.Older;
        }

        #endregion

        #region Constructors

        protected CallbackRequestsBaseViewModel(
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
            ConnectionStatusManager connectionStatusManager) : base(
            navigationService, connectivity, gsmConnection, pageDialogService, smsTask, doctorRepository, unitOfWork,
            crashReporter, callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            CallbackRequestRepository = callbackRequestRepository;

            DoctorRepository = doctorRepository;

            Connectivity = connectivity;

            CallbackRequests = new ObservableCollection<CallbackRequestGroupList>();
        }

        #endregion

        #region Life-cycle methods

        public override void OnAppearing()
        {
            base.OnAppearing();

            LoadCallbackRequests();
        }

        #endregion

        #region Public Properties

        public ObservableCollection<CallbackRequestGroupList> CallbackRequests
        {
            get => _callbackRequests;
            set => SetProperty(ref _callbackRequests, value);
        }

        public bool IsCallbackRequestsEmpty
        {
            get => _isCallbackRequestsEmpty;
            set => SetProperty(ref _isCallbackRequestsEmpty, value);
        }

        #endregion

        #region Protected Methods

        protected IDoctorRepository DoctorRepository;

        protected IConnectivity Connectivity;

        protected void LoadCallbackRequests()
        {
            var callbackRequests = GetCallbackRequests();

            OrganizeCallbackRequests(callbackRequests?.ToList());

            IsCallbackRequestsEmpty = (CallbackRequests == null || CallbackRequests.Count < 1);
        }

        protected async Task GetLatestCallbackRequestsFromServerAsync(ServerCallbackRequestsFetchType fetchType)
        {
            if (!Connectivity.IsConnected)
                return;

            var doctor = DoctorRepository.Get();

            if (doctor == null)
                return;

            ICollection<CallbackRequest> callbackRequests = null;

            try
            {
                if (fetchType == ServerCallbackRequestsFetchType.Active)
                {
                    callbackRequests = await doctor.GetActiveCallbackRequestsAsync();
                }
                else
                {
                    callbackRequests = await doctor.GetAllCallbackRequestsAsync();
                }
                
            }
            catch (Exception exception)
            {
                CrashReporter.SendException(exception);
            }

            if (callbackRequests == null || !callbackRequests.Any())
                return;

            SyncLocalDbCallbackRequestsWithServer(callbackRequests);
        }

        #endregion

        #region Abstract Methods

        protected abstract IEnumerable<CallbackRequestBindableObject> GetCallbackRequests();

        protected abstract IOrderedEnumerable<CallbackRequestBindableObject> OrderCallbackRequests(IEnumerable<CallbackRequestBindableObject> callbackRequests);

        #endregion

        #region Private Methods

        private void OrganizeCallbackRequests(IList<CallbackRequestBindableObject> callbackRequests)
        {
            if (callbackRequests == null || !callbackRequests.Any())
                return;

            var todayCallbackRequests = new CallbackRequestGroupList
            { GroupTitle = Resources.Today, GroupIndex = 0, Group = CallbackRequestGroupList.CallbackRequestGroup.Today };

            var yesterdayCallbackRequests = new CallbackRequestGroupList
            { GroupTitle = Resources.Yesterday, GroupIndex = 1, Group = CallbackRequestGroupList.CallbackRequestGroup.Yesterday };

            var olderCallbackRequests = new CallbackRequestGroupList
            { GroupTitle = Resources.Older, GroupIndex = 2, Group = CallbackRequestGroupList.CallbackRequestGroup.Older };

            var sortedCallbackRequests = OrderCallbackRequests(callbackRequests);

            foreach (var callbackRequest in sortedCallbackRequests)
            {
                var group = CalculateCallbackRequestGroup(callbackRequest);

                switch (group)
                {
                    case CallbackRequestGroupList.CallbackRequestGroup.Today:
                        {
                            todayCallbackRequests.Add(callbackRequest);
                            break;
                        }
                    case CallbackRequestGroupList.CallbackRequestGroup.Yesterday:
                        {
                            yesterdayCallbackRequests.Add(callbackRequest);
                            break;
                        }
                    case CallbackRequestGroupList.CallbackRequestGroup.Older:
                        {
                            olderCallbackRequests.Add(callbackRequest);
                            break;
                        }
                    default:
                        {
                            continue;
                        }
                }
            }

            var groups = new ObservableCollection<CallbackRequestGroupList>();

            if (todayCallbackRequests.Any())
            {
                groups.Add(todayCallbackRequests);
            }

            if (yesterdayCallbackRequests.Any())
            {
                groups.Add(yesterdayCallbackRequests);
            }

            if (olderCallbackRequests.Any())
            {
                groups.Add(olderCallbackRequests);
            }

            if (!todayCallbackRequests.Any() && !yesterdayCallbackRequests.Any())
            {
                var theGroup = groups.FirstOrDefault();

                if (theGroup != null)
                    theGroup.GroupTitle = Resources.CallbackRequests;
            }

            CallbackRequests = groups;
        }

        private void SyncLocalDbCallbackRequestsWithServer(ICollection<CallbackRequest> serverCallbackRequests)
        {
            foreach (var serverCallbackRequest in serverCallbackRequests)
            {
                var dbCallbackRequest = CallbackRequestRepository.Get(serverCallbackRequest.Id);

                if (dbCallbackRequest == null)
                {
                    CallbackRequestRepository.Add(serverCallbackRequest);
                }
                else
                {
                    //we consider callback request's ConsentGivenAt(i.e its latest updated dateTime)
                    //as a point to check if a callback request is changed.
                    if (serverCallbackRequest.ConsentGivenAt != dbCallbackRequest.ConsentGivenAt)
                    {
                        dbCallbackRequest.ResetAppSideLocalValues();
                    }

                    DoctorAppAutoMapper.Instance.Map(serverCallbackRequest, dbCallbackRequest);

                    CallbackRequestRepository.Update(dbCallbackRequest);
                }
            }

            UnitOfWork.Commit();

            LoadCallbackRequests();
        }

        #endregion

        #region Fields

        private ObservableCollection<CallbackRequestGroupList> _callbackRequests;
        protected readonly ICallbackRequestRepository CallbackRequestRepository;
        private bool _isCallbackRequestsEmpty = false;

        #endregion

        #region Inner Types

        public class CallbackRequestGroupList : ObservableCollection<CallbackRequestBindableObject>
        {
            public string GroupTitle { get; set; }
            public int GroupIndex { get; set; }
            public CallbackRequestGroup Group { get; set; }

            public enum CallbackRequestGroup
            {
                Undefined,
                Today,
                Yesterday,
                Older,
                LatestSuccessfulCalls
            }
        }

        protected enum ServerCallbackRequestsFetchType
        {
            All = 0,
            Active = 1
        }

        #endregion
    }

    //After successfully establishing a call back request and returning back to page (CallBackRequestsPage),
    //the previous successful established call back request is removed from the list and this new one replaces previous one.
    public delegate Task CallbackRequestDeleteRequestedHandler();

    /// <summary>
    /// This is wrapper class to insert CallbackRequest object and also to add further logic that only needed
    /// by pages. So instead of adding these logic to CallbackRequest(which is an entity) and making it dirty, we wrap
    /// it by using following class.
    /// This further logic I mentioned above includes a bindable flag used for indicating if a CallbackRequest can ba called back,
    /// and also an animation will be run for removing the one-to-last successful callback request.
    /// </summary>
    public class CallbackRequestBindableObject : BindableObject
    {
        public static readonly BindableProperty IsCallEnableProperty = BindableProperty.Create(nameof(IsCallEnable),
            typeof(bool), typeof(CallbackRequestBindableObject), defaultValue: true);

        public event CallbackRequestDeleteRequestedHandler OnCallbackRequestDeleteRequested;

        public CallbackRequestBindableObject(CallbackRequest callbackRequest)
        {
            CallbackRequest = callbackRequest;
        }

        public CallbackRequest CallbackRequest { get; set; }

        public bool IsCallEnable
        {
            get => (bool)GetValue(IsCallEnableProperty);
            set => SetValue(IsCallEnableProperty, value);
        }

        public async Task RunRemoveAnimation()
        {
            if (OnCallbackRequestDeleteRequested != null)
                await OnCallbackRequestDeleteRequested?.Invoke();
        }
    }
}