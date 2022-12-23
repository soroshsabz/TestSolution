using BSN.Resa.Core.Commons.Validators;
using BSN.Resa.DoctorApp.Aspects;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.DoctorApp.Views;
using BSN.Resa.Locale;
using Microsoft.AppCenter;
using Plugin.Connectivity.Abstractions;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        #region Constructors

        public LoginPageViewModel(
            IMobileContactManager mobileContactManager,
            INavigationService navigationService,
            IConnectivity connectivity,
            IPageDialogService pageDialogService,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IPermissionsManager permissionsManager,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IMedicalTestRepository medicalTestRepository,
            IConfig config)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;
            _crashReporter = crashReporter;
            _callbackRequestRepository = callbackRequestRepository;
            _medicalTestRepository = medicalTestRepository;
            _config = config;
            _permissionsManager = permissionsManager;
            _mobileContactManager = mobileContactManager;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;

            SetPageHeader();
        }

        #endregion

        #region Bound Properties

        public bool IsConnectingToServer
        {
            get => _isConnectingToServer;
            set => SetProperty(ref _isConnectingToServer, value);
        }

        public ICommand LoginCommand => new Command(async () => { await Login(); });

        public string Vsin
        {
            get => _vsin;
            set => SetProperty(ref _vsin, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// It's the page heading.
        /// On Android and iOS local markets version it's:
        /// Farsi: رسالت سلامت ایرانیان
        /// English: Iranians Health Media
        ///
        /// On iOS normal version it's:
        /// Farsi: رسالت سلامت عمانیان
        /// English: Resaa
        /// </summary>
        public string ResaFullName
        {
            get => _resaFullName;
            set => SetProperty(ref _resaFullName, value);
        }

        #endregion

        #region Private Methods

        [CentralizedExceptionHandler]
        private async Task Login()
        {
            if (!await ValidateLoginPreconditions())
                return;

            try
            {
                Doctor doctor = _doctorRepository.Get();

                IsConnectingToServer = true;

                await doctor.AuthenticateAsync(Vsin, Password);

                await doctor.LoginAsync();

                var tasks = new List<Task>
                {
                    GetAndSaveCallbackRequestPhoneNumber(doctor),
                    GetCallbackRequestsFromServerAndSaveInDb(doctor),
                    GetMedicalTestsFromServerAndSaveInDb(doctor)
                };

                await Task.WhenAll(tasks);

                IsConnectingToServer = false;

                _doctorRepository.Update();

                _unitOfWork.Commit();

                AddResaContactToDevice(doctor);

                SaveAndSetAppCenterUserId();

                DoctorAppSettings.IsDoctorLoggedIn = true;

                await _navigationService.NavigateAsync($"/{nameof(PermissionsPage)}");
            }
            catch (InternetNotAvailableException)
            {
                IsConnectingToServer = false;

                await ShowInternetNotAvailableDialogAsync();
            }
            catch (AuthenticationException)
            {
                IsConnectingToServer = false;

                await ShowAuthFailedDialogAsync();
            }
            catch (Exception)
            {
                IsConnectingToServer = false;

                ResetDatabaseCaching();

                throw;
            }
            finally
            {
                IsConnectingToServer = false;
            }
        }

        private async Task GetCallbackRequestsFromServerAndSaveInDb(Doctor doctor)
        {
            var callbackRequests = await doctor.GetAllCallbackRequestsAsync();

            if (callbackRequests != null && callbackRequests.Any())
            {
                ResetEstablishedCallbackRequestsStates(callbackRequests);
                _callbackRequestRepository.AddOrUpdateRange(callbackRequests);
            }
        }

        private async Task GetMedicalTestsFromServerAndSaveInDb(Doctor doctor)
        {
            var medicalTests = await doctor.GetAllActiveMedicalTests();
            _medicalTestRepository.AddOrUpdateRange(medicalTests);
        }

        private async Task<bool> ValidateLoginPreconditions()
        {
            if (!await ValidateVsinAndPasswordLookWise())
                return false;

            if (!await ValidateVsinContentWise())
                return false;

            if (!await ValidateConnectivity())
                return false;

            if (!await _permissionsManager.TryGetContactsPermissionAsync())
                return false;

            return true;
        }

        private async Task<bool> ValidateConnectivity()
        {
            if (!_connectivity.IsConnected)
            {
                await ShowInternetNotAvailableDialogAsync();

                return false;
            }

            return true;
        }

        private async Task<bool> ValidateVsinContentWise()
        {
            if (!VSINValidator.IsValid(Vsin))
            {
                await _pageDialogService.DisplayAlertAsync(Resources.InputError, Resources.VSINInvalid, Resources.Retry);

                Password = string.Empty;

                return false;
            }

            return true;
        }

        private async Task<bool> ValidateVsinAndPasswordLookWise()
        {
            if (string.IsNullOrEmpty(Vsin) || string.IsNullOrEmpty(Password))
            {
                await _pageDialogService.DisplayAlertAsync(Resources.InputError, Resources.EmptyUsernameOrPasswordError, Resources.Retry);

                return false;
            }

            return true;
        }

        private void ResetEstablishedCallbackRequestsStates(IEnumerable<CallbackRequest> callbackRequests)
        {
            foreach (var callbackRequest in callbackRequests)
            {
                if (callbackRequest.ReturnCallHasBeenEstablished)
                {
                    callbackRequest.IsEstablishedCallNotified = true;
                    callbackRequest.IsSeen = true;
                }
            }
        }

        private async Task ShowAuthFailedDialogAsync()
        {
            await _pageDialogService.DisplayAlertAsync(Resources.LoginError,
                Resources.InvalidUsernameOrPassword,
                Resources.Retry);
            Password = "";
        }

        private async Task ShowInternetNotAvailableDialogAsync()
        {
            string selectedAction = await _pageDialogService.DisplayActionSheetAsync(
                Resources.NoConnectionError, null, null, Resources.Retry, Resources.Cancel);
            if (selectedAction == Resources.Retry)
                await Login();
        }

        private void AddResaContactToDevice(Doctor doctor)
        {
            try
            {
                _mobileContactManager.AddContact(
                    Resources.Resa,
                    string.Empty,
                    doctor.Contacts
                        .Where(c => c.IsResaContact)
                        .Select(c => c.PhoneNumber)
                        .ToArray(),
                    "Assets/resa_contact.png"
                );
            }
            catch (ServiceCommunicationException)
            {
                // ignored
            }
        }

        private void SaveAndSetAppCenterUserId()
        {
            string userId = _doctorRepository.Get()?.Msisdn;

            if (userId.IsNullOrEmptyOrSpace())
            {
                _crashReporter.SendException(new Exception("Could not set AppCenter UserId!"));
                return;
            }

            DoctorAppSettings.AppCenterUserId = userId;

            AppCenter.SetUserId(userId);
        }

        private async Task GetAndSaveCallbackRequestPhoneNumber(Doctor doctor)
        {
            string phoneNumber = await doctor.GetCallbackRequestPhoneNumber();

            if (!phoneNumber.IsNullOrEmptyOrSpace())
                DoctorAppSettings.CallbackRequestPhoneNumber = phoneNumber;
        }

        private void SetPageHeader()
        {
            ResaFullName = _config.AppFullTitle;
        }

        private void ResetDatabaseCaching()
        {
            _callbackRequestRepository = DependencyInjectionHelper.Resolve<ICallbackRequestRepository>();
            _medicalTestRepository = DependencyInjectionHelper.Resolve<IMedicalTestRepository>();
        }

        #endregion

        #region Fields

        private string _vsin;
        private string _password;
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;
        private readonly ICrashReporter _crashReporter;
        private ICallbackRequestRepository _callbackRequestRepository;
        private IMedicalTestRepository _medicalTestRepository;
        private readonly IConfig _config;
        private readonly IPermissionsManager _permissionsManager;
        private readonly IMobileContactManager _mobileContactManager;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private bool _isConnectingToServer;
        private string _resaFullName;

        #endregion
    }
}