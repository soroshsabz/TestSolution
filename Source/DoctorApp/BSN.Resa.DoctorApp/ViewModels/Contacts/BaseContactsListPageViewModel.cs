using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Aspects;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BSN.Resa.DoctorApp.Services;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.ViewModels.Contacts
{
    /// <summary>
    /// This is abstract class for reduce duplication code between phone number list pages. do not use this class directly
    /// </summary>
    public abstract class BaseContactsListPageViewModel : BaseViewModel
    {
        #region Inner Classes

        public class ContactItem
        {
            public string PhoneNumber { get; set; }

            public int BlockedCount { get; set; } = 0;

            public ICommand RemoveContactCommand { get; set; }

            public bool IsVisible { get; set; } = true;
        }

        #endregion

        #region Constructors

        protected BaseContactsListPageViewModel(
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IConnectivity connectivity,
            ISmsTask smsTask,
            ICrashReporter crashReporter,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            IUserDialogs userDialogs,
            INavigationService navigationService,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager
            ) : base(navigationService, connectivity, gsmConnection,
            pageDialogService, smsTask, doctorRepository, unitOfWork, crashReporter,
            callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            DoctorRepository = doctorRepository;
            PageDialogService = pageDialogService;
            UserDialogs = userDialogs;

            Contacts = new ObservableCollection<ContactItem>();
            AddContactCommand = new DelegateCommand(async () => await AddContactAsync());
            StartSearchCommand = new DelegateCommand(StartSearch);
            CancelSearchCommand = new DelegateCommand(CancelSearch);
            IsSearchMode = false;
        }

        #endregion

        #region Properties

        public ObservableCollection<ContactItem> Contacts { get; set; }

        public ObservableCollection<ContactItem> VisibleContacts =>
            new ObservableCollection<ContactItem>(Contacts.Where(c => c.IsVisible));

        public ICommand AddContactCommand { get; set; }

        public ICommand StartSearchCommand { get; }

        public ICommand CancelSearchCommand { get; }

        public bool IsSearchMode
        {
            get => _isSearchMode;
            set
            {
                SetProperty(ref _isSearchMode, value);
                if (value == false)
                    SearchingPhoneNumber = string.Empty;
            }
        }

        public string SearchingPhoneNumber
        {
            get => _searchingPhoneNumber;
            set
            {
                SetProperty(ref _searchingPhoneNumber, value);
                if (IsSearchMode && !string.IsNullOrEmpty(value))
                {
                    Contacts.ToList().ForEach(c => c.IsVisible = c.PhoneNumber.Contains(value));
                }
                else
                {
                    Contacts.ToList().ForEach(c => c.IsVisible = true);
                }

                RefreshList();
            }
        }

        #endregion

        #region Protected Methods

        [CentralizedExceptionHandler]
        protected async Task RemoveContactAsync(string phoneNumber)
        {

            string selectedAction = await PageDialogService.DisplayActionSheetAsync(
                Resources.AreYouSureYouWantDeleteNumber,
                null, null, Resources.Yes, Resources.Cancel);

            if (selectedAction == null || selectedAction == Resources.Cancel)
                return;

            Contacts.Remove(Contacts.First(c => c.PhoneNumber == phoneNumber));
            RefreshList();

            Doctor doctor = DoctorRepository.Get();

            await doctor.RemoveContactAsync(phoneNumber);

            DoctorRepository.Update();
            UnitOfWork.Commit();
        }

        protected abstract Task OnContactAddedAsync(string phoneNumber);

        protected abstract void LoadContacts();

        protected void RefreshList()
        {
            RaisePropertyChanged(nameof(VisibleContacts));
        }

        #endregion

        #region  Private Methods

        [CentralizedExceptionHandler]
        private async Task AddContactAsync()
        {
            PromptResult result = await UserDialogs.PromptAsync(new PromptConfig()
            {
                InputType = InputType.Phone,
                Message = GetAddPhoneNumberDialogMessage(),
                OkText = Resources.Add,
                CancelText = Resources.Cancel,
                MaxLength = 20 //could be even more precise
            });

            Doctor doctor = DoctorRepository.Get();
            if (result.Ok)
            {

                string phoneNumber = result.Value;

                bool isPhoneNumberValid = phoneNumber.IsValidPhoneNumber();

                if (isPhoneNumberValid)
                    phoneNumber = phoneNumber.ToE164PhoneNumberFormat();

                if (!isPhoneNumberValid ||
                    doctor.Contacts.Any(c => c.PhoneNumber == phoneNumber && c.IsResaContact))
                {
                    await PageDialogService.DisplayAlertAsync(Resources.InputError, Resources.InputPhoneNumberError,
                        Resources.Ok);
                }
                else
                {
                    await OnContactAddedAsync(phoneNumber);
                }
            }
        }

        private void StartSearch()
        {
            IsSearchMode = true;
        }

        private void CancelSearch()
        {
            IsSearchMode = false;
        }

        private static string GetAddPhoneNumberDialogMessage()
        {
            return Device.RuntimePlatform == Device.iOS
                ? Resources.PleaseEnterTheFullPhoneNumberWithCountryCode
                : Resources.PleaseEnterThePhoneNumber;
        }

        #endregion

        #region Fields

        private string _searchingPhoneNumber;
        private bool _isSearchMode;
        protected readonly IDoctorRepository DoctorRepository;
        protected readonly IPageDialogService PageDialogService;
        protected readonly IUserDialogs UserDialogs;

        #endregion
    }
}