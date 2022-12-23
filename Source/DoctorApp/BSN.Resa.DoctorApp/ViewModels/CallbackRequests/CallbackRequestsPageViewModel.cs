using BSN.Resa.DoctorApp.Aspects;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Views.Utilities;
using BSN.Resa.Locale;
using Plugin.Badge.Abstractions;
using Plugin.Connectivity.Abstractions;
using Plugin.LocalNotifications.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels.CallbackRequests
{
    public class CallbackRequestsPageViewModel : CallbackRequestsBaseViewModel
    {
        #region Constructor

        public CallbackRequestsPageViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            ICallbackRequestRepository callbackRequestRepository,
            IPhoneCallTask phoneCallTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            IBadge appIconBadge,
            ILocalNotifications localNotifications,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager
        ) : base(navigationService, connectivity, gsmConnection, pageDialogService, smsTask, callbackRequestRepository,
            doctorRepository, unitOfWork, permissionsManager, crashReporter, connectionStatusManager)
        {
            _appIconBadge = appIconBadge;
            _localNotifications = localNotifications;
            _newlySeenCallbacks = new ObservableCollection<CallbackRequest>();
            _phoneCallTask = phoneCallTask;
            _pageDialogService = pageDialogService;

            SetCallingAvailability();
        }

        #endregion

        #region Life-cycle methods

        public override async void OnAppearing()
        {
            base.OnAppearing();

            _localNotifications.Cancel(CallbackRequestNotificationId);
            _appIconBadge.ClearBadge();

            await GetLatestCallbackRequestsFromServerAsync(ServerCallbackRequestsFetchType.Active);

            await NotifySecondSuccessfulCallIfExistsAsync();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            UpdateNewlySeenCallbacksStates();

            UnitOfWork.Commit();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            UpdateNewlySeenCallbacksStates();

            UnitOfWork.Commit();
        }

        #endregion

        #region Public Properties

        public ICommand OnItemAppearingCommand => new Command(args =>
        {
            var callbackRequestWrapper = args as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;
            if (callbackRequest == null) return;

            if (!callbackRequest.IsSeen)
            {
                callbackRequest.IsSeen = true;
                _newlySeenCallbacks.Add(callbackRequest);
                UnSeenCallbackCount--;
            }
        });

        public bool IsCallEnable
        {
            get => _isCallEnable;
            set => SetProperty(ref _isCallEnable, value);
        }

        public ICommand CallCommand => new Command(async arg =>
        {
            var selectedCallbackRequestWrapper = arg as CallbackRequestBindableObject;
            var selectedCallback = selectedCallbackRequestWrapper?.CallbackRequest;

            if (selectedCallback == null || selectedCallback.Id.IsNullOrEmptyOrSpace()) return;

            await Call(selectedCallback);
        });

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion

        #region Overridden Stuff

        protected override IEnumerable<CallbackRequestBindableObject> GetCallbackRequests()
        {
            var activeCallbackRequests = CallbackRequestRepository.GetActiveCallbackRequests();

            if (activeCallbackRequests == null)
                return ImmutableArray<CallbackRequestBindableObject>.Empty;

            var callbackRequestWrappers = activeCallbackRequests.ToCallbackRequestWrappers();

            var latestTwoUnNotifiedSuccessfulCalls = GetLatestSuccessfulCalls()?.ToList();

            if (latestTwoUnNotifiedSuccessfulCalls != null && latestTwoUnNotifiedSuccessfulCalls.Any())
                callbackRequestWrappers.AddRange(latestTwoUnNotifiedSuccessfulCalls);

            return callbackRequestWrappers;
        }

        protected override IOrderedEnumerable<CallbackRequestBindableObject> OrderCallbackRequests(IEnumerable<CallbackRequestBindableObject> callbackRequests)
        {
            return callbackRequests.OrderByDescending(call => call.CallbackRequest.ReturnCallHasBeenEstablished)
                .ThenBy(call => call.CallbackRequest.IsCallTried).ThenBy(call => call.CallbackRequest.LastCallTriedAt)
                .ThenByDescending(call => call.CallbackRequest.ConsentGivenAt);
        }

        protected override bool HasPageChangingDoctorStateFeature { get;} = true;

        #endregion

        #region Private Methods

        private void UpdateNewlySeenCallbacksStates()
        {
            if (_newlySeenCallbacks != null && _newlySeenCallbacks.Any())
            {
                CallbackRequestRepository.UpdateRange(_newlySeenCallbacks);
            }
        }

        private IEnumerable<CallbackRequestBindableObject> GetLatestSuccessfulCalls()
        {
            var callsRequests = CallbackRequestRepository.GetMany(call =>
                    call.ReturnCallHasBeenEstablished && !call.IsEstablishedCallNotified)?
                .OrderByDescending(call => call.ConsentGivenAt).Take(2).ToList();

            var callbackRequestWrappers = callsRequests.ToCallbackRequestWrappers();

            foreach (var callbackRequestWrapper in callbackRequestWrappers ?? Enumerable.Empty<CallbackRequestBindableObject>())
            {
                callbackRequestWrapper.IsCallEnable = false;
            }

            return callbackRequestWrappers;
        }

        private async Task NotifySecondSuccessfulCallIfExistsAsync()
        {
            //making some delay so that user can be able to see and notice that the previous
            //successful callback request was deleted in an animation manner. Without this delay this operation will happen
            //very immediately and user can't see it.
            await Task.Delay(TimeSpan.FromSeconds(2));

            var lastTwoSuccessfulCalls = GetLatestSuccessfulCalls()?.ToList();

            if (lastTwoSuccessfulCalls == null || !lastTwoSuccessfulCalls.Any() ||
                lastTwoSuccessfulCalls.Count == 1) return;

            var lastSuccessfulCall = lastTwoSuccessfulCalls.LastOrDefault();

            if (lastSuccessfulCall == null) return;

            //getting this callbackRequest's group(A.K.A category) so that we can find it in the
            //CallbackRequests bindable collection which is nested!
            var callbackRequestGroup = CalculateCallbackRequestGroup(lastSuccessfulCall);

            var callbackRequestGroupCollection =
                CallbackRequests?.FirstOrDefault(group => group.Group == callbackRequestGroup) as
                    ObservableCollection<CallbackRequestBindableObject>;

            var theCallRequest =
                callbackRequestGroupCollection?.FirstOrDefault(call => call.CallbackRequest.Id == lastSuccessfulCall.CallbackRequest.Id);

            if (theCallRequest == null) return;

            await theCallRequest.RunRemoveAnimation();

            var dbCallbackRequest = CallbackRequestRepository.Get(theCallRequest.CallbackRequest.Id);
            dbCallbackRequest.IsEstablishedCallNotified = true;

            CallbackRequestRepository.Update(dbCallbackRequest);

            callbackRequestGroupCollection.Remove(theCallRequest);
        }

        [CentralizedExceptionHandler]
        private async Task Call(CallbackRequest callbackRequest)
        {
            if (!Connectivity.IsConnected)
            {
                await ShowInternetNotAvailableAlertAsync();

                return;
            }

            IsBusy = true;

            try
            {
                await callbackRequest.BookAsync();
            }
            catch (PaymentRequiredException)
            {
                await DisplayPatientHasInsufficientCreditMessage();

                return;
            }
            catch (ServiceCommunicationException serviceCommunicationException)
            {
                if (serviceCommunicationException.HttpStatusCode != HttpStatusCode.Conflict)
                {
                    await ShowAppInternalErrorAlertAsync();
                    return;
                }

                //else: continue because conflict here means this callback booked more than once, so
                //we ignore it and continue.
            }
            catch (Exception)
            {
                IsBusy = false;

                throw;
            }
            finally
            {
                IsBusy = false;
            }

            callbackRequest.IsCallTried = true;
            callbackRequest.LastCallTriedAt = DateTime.Now;

            CallbackRequestRepository.AddOrUpdate(callbackRequest);

            await MakeThePhoneCall();
        }

        private async Task DisplayPatientHasInsufficientCreditMessage()
        {
            await _pageDialogService.DisplayAlertAsync(Resources.Error,
                Resources.DoctorAppPatientInsufficientCreditMessage, Resources.Ok);
        }

        private async Task MakeThePhoneCall()
        {
            bool isCallPermissionGranted = await PermissionsManager.IsPhonePermissionGrantedAsync();

            if (_phoneCallTask.CanMakePhoneCall && isCallPermissionGranted)
            {
                _phoneCallTask.MakePhoneCall(RESA_CALLBACK_REQUEST_PHONE_NUMBER);
            }
            else
            {
                await Launcher.OpenAsync(new Uri($"tel:{RESA_CALLBACK_REQUEST_PHONE_NUMBER}"));
            }
        }

        private void SetCallingAvailability()
        {
            IsCallEnable = !RESA_CALLBACK_REQUEST_PHONE_NUMBER.IsNullOrEmptyOrSpace();
        }

        #endregion

        #region Private Fields

        private readonly ObservableCollection<CallbackRequest> _newlySeenCallbacks;
        private readonly IBadge _appIconBadge;
        private readonly ILocalNotifications _localNotifications;
        private readonly IPhoneCallTask _phoneCallTask;
        private bool _isBusy;
        private bool _isCallEnable;
        private readonly IPageDialogService _pageDialogService;
        private const int CallbackRequestNotificationId = 2001;//2001 is the id of callback request notification
        private static readonly string RESA_CALLBACK_REQUEST_PHONE_NUMBER = DoctorAppSettings.CallbackRequestPhoneNumber;
        #endregion
    }
}