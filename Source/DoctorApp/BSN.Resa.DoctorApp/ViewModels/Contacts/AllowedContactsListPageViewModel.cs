using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
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
using System.Linq;
using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Services;

namespace BSN.Resa.DoctorApp.ViewModels.Contacts
{
    public class AllowedContactsListPageViewModel : BaseContactsListPageViewModel
    {
        #region Constructors

        public AllowedContactsListPageViewModel(
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IConnectivity connectivity,
            ISmsTask smsTask,
            IPageDialogService pageDialogService,
            IUserDialogs userDialogs,
            IGsmConnection gsmConnection,
            ICrashReporter crashReporter,
            INavigationService navigationService,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager)
            : base(doctorRepository, unitOfWork, connectivity, smsTask, crashReporter, gsmConnection,
                pageDialogService, userDialogs, navigationService,
                callbackRequestRepository, permissionsManager, connectionStatusManager)
        {
            LoadContacts();
        }

        #endregion

        #region Protected Methods

        protected override async Task OnContactAddedAsync(string phoneNumber)
        {
            Doctor doctor = DoctorRepository.Get();

            if (doctor.Contacts.Any(c => c.PhoneNumber == phoneNumber && !c.IsBlocked))
            {
                await PageDialogService.DisplayAlertAsync(Resources.DuplicateNumber,
                    Resources.InsertedNumberIsAlreadyInAllowedList, Resources.Ok);
                return;
            }

            await doctor.AddOrUpdateContactAsync(Contact.Allowed(phoneNumber, isSynchronized: false));

            DoctorRepository.Update();
            UnitOfWork.Commit();

            Contacts.Add(new ContactItem
            {
                PhoneNumber = phoneNumber,
                RemoveContactCommand = new DelegateCommand(async () => await RemoveContactAsync(phoneNumber)),
            });
            RefreshList();
        }

        protected sealed override void LoadContacts()
        {
            foreach (Contact contact in DoctorRepository.Get().Contacts.Where(c => !c.IsBlocked && c.IsVisible))
            {
                Contacts.Add(new ContactItem()
                {
                    PhoneNumber = contact.PhoneNumber,
                    RemoveContactCommand =
                        new DelegateCommand(async () => await RemoveContactAsync(contact.PhoneNumber)),
                });
            }

            RefreshList();
        }

        protected override bool HasPageChangingDoctorStateFeature { get; } = true;

        #endregion
    }
}