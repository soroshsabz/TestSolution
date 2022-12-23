using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.Locale;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Linq;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.ViewModels.Contacts
{
    public class BlockedContactsListPageViewModel : BaseContactsListPageViewModel
    {
        #region Constructors

        public BlockedContactsListPageViewModel(
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IConnectivity connectivity,
            IPageDialogService pageDialogService,
            IUserDialogs userDialogs,
            ISmsTask smsTask,
            ICrashReporter crashReporter,
            IGsmConnection gsmConnection,
            ICallBlockAndIdentification callBlockAndIdentification,
            INavigationService navigationService,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager)
            : base(doctorRepository, unitOfWork, connectivity, smsTask, crashReporter, gsmConnection,
                pageDialogService, userDialogs, navigationService, callbackRequestRepository, permissionsManager,
                connectionStatusManager)
        {
            _callBlockAndIdentification = callBlockAndIdentification;
            _navigationService = navigationService;

            LoadContacts();
        }

        #endregion

        #region Protected Methods

        protected override async Task OnContactAddedAsync(string phoneNumber)
        {
            Doctor doctor = DoctorRepository.Get();

            if (doctor.Contacts.Any(c => c.PhoneNumber == phoneNumber && c.IsBlocked))
            {
                await PageDialogService.DisplayAlertAsync(Resources.DuplicateNumber,
                    Resources.InsertedNumberIsAlreadyInBlockedList, Resources.Ok);
                return;
            }

            // Checking if SMS permission is granted, otherwise trying to get it,
            // because if internet is not available we send blocked number via sms.
            // For more info see DoctorServiceCommunicatorViaSms class.
            await PermissionsManager.TryGetSmsPermissionAsync();

            await doctor.AddOrUpdateContactAsync(Contact.Blocked(phoneNumber, isSynchronized: false));

            DoctorRepository.Update();
            UnitOfWork.Commit();

            Contacts.Add(new ContactItem
            {
                BlockedCount = 0,
                PhoneNumber = phoneNumber,
                RemoveContactCommand = new DelegateCommand(async () => await RemoveContactAsync(phoneNumber)),
            });

            RefreshList();

            if (!_callBlockAndIdentification.IsBlockingEnabled)
            {
                await PageDialogService.DisplayAlertAsync(Resources.CallBlockingPermission,
                    Resources.ResaHasNotCallBlockingPermissionMessage,
                    Resources.GoToHelp);
                await _navigationService.NavigateAsync("/FlyoutPage/AppNavigationPage/HelpPage");
            }
        }

        protected sealed override void LoadContacts()
        {
            foreach (Contact contact in DoctorRepository.Get().Contacts.Where(c => c.IsBlocked && c.IsVisible))
            {
                Contacts.Add(new ContactItem()
                {
                    PhoneNumber = contact.PhoneNumber,
                    RemoveContactCommand =
                        new DelegateCommand(async () => await RemoveContactAsync(contact.PhoneNumber)),
                    BlockedCount = contact.BlockedCount,
                });
            }

            RefreshList();
        }

        protected override bool HasPageChangingDoctorStateFeature { get; } = true;

        #endregion

        #region Fields

        private readonly ICallBlockAndIdentification _callBlockAndIdentification;
        private readonly INavigationService _navigationService;

        #endregion
    }
}