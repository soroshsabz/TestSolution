using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Domain.Models
{
    public delegate void OnContactsChanged(IReadOnlyCollection<Contact> contacts);

    public partial class Doctor : User
    {
        #region Constructor

        public Doctor(IApplicationServiceCommunicator applicationServiceCommunicator,
            IDoctorServiceCommunicator doctorServiceCommunicator)
            : this()
        {
            ApplicationServiceCommunicator = applicationServiceCommunicator;
            DoctorServiceCommunicator = doctorServiceCommunicator;
        }

        #endregion Constructor

        #region Properties

        public string Msisdn { get; protected set; }

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

        public Commons.DoctorState State { get; protected set; }

        public IReadOnlyCollection<Contact> Contacts => InternalContacts.AsReadOnly();

        public string Vsin => Msisdn?.Substring(Msisdn.Length - Config.VsinLength);

        public string DoctorVsin => Msisdn?.Substring(Msisdn.Length - Config.DoctorVsinLength);

        public IDoctorServiceCommunicator DoctorServiceCommunicator { get; set; }

        protected List<Contact> InternalContacts { get; set; }

        #endregion Properties

        #region Events

        public static event OnContactsChanged OnContactsChanged;

        public event OnSendDoctorStateResult OnSendDoctorStateResult;

        #endregion Events

        #region Methods

        public async Task LoginAsync()
        {
            SetServiceCommunicatorToken();

            await UpdateAsync(false).ConfigureAwait(false);

            var tasks = new List<Task<string[]>>
            {
                DoctorServiceCommunicator.GetAllowedPhoneNumbersAsync(),
                DoctorServiceCommunicator.GetBlockedPhoneNumbersAsync(),
                DoctorServiceCommunicator.GetResaPhoneNumbersAsync()
            };

            var aggregatedTask = Task.WhenAll(tasks);

            var results = await aggregatedTask.ConfigureAwait(false);

            if (aggregatedTask.Status != TaskStatus.RanToCompletion)
                return;

            string[] allowedPhoneNumbers = results[0];
            string[] blockedPhoneNumbers = results[1];
            string[] resaPhoneNumbers = results[2];

            InternalContacts.Clear();

            InternalContacts.AddRange(allowedPhoneNumbers.Select(p => Contact.Allowed(p, isSynchronized: true)));
            InternalContacts.AddRange(blockedPhoneNumbers.Select(p => Contact.Blocked(p, isSynchronized: true)));
            InternalContacts.AddRange(resaPhoneNumbers.Select(Contact.Resa));

            TriggerContactsChanged();

            IsLoggedIn = true;
        }

        public Task UpdateAsync()
        {
            return UpdateAsync(true);
        }

        public DoctorServiceCommunicationToken GetServiceCommunicatorToken()
        {
            return DoctorServiceCommunicationToken.Create(ServiceCommuncationToken, Vsin);
        }

        public async Task AddOrUpdateContactAsync(Contact addingContact)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            async Task<bool> SendContactToServiceIfNotAnnounced(Contact newContact)
            {
                try
                {
                    if (!newContact.IsAnnouncedToService)
                    {
                        if (newContact.IsBlocked)
                            await DoctorServiceCommunicator
                                .SendBlockedPhoneNumbersAsync(new string[] { addingContact.PhoneNumber })
                                .ConfigureAwait(false);
                        else
                            await DoctorServiceCommunicator
                                .SendAllowedPhoneNumbersAsync(new string[] { addingContact.PhoneNumber })
                                .ConfigureAwait(false);
                    }

                    return true;
                }
                catch (ServiceCommunicationException)
                {
                    return false;
                }
            }

            Contact preContact = Contacts.FirstOrDefault(c => c.PhoneNumber == addingContact.PhoneNumber);

            if (preContact == null)
            {
                InternalContacts.Add(addingContact);
                addingContact.IsAnnouncedToService =
                    await SendContactToServiceIfNotAnnounced(addingContact).ConfigureAwait(false);
            }
            else
            {
                preContact.Update(addingContact);
                preContact.IsAnnouncedToService =
                    await SendContactToServiceIfNotAnnounced(preContact).ConfigureAwait(false);
            }

            TriggerContactsChanged();
        }

        public async Task AddOrUpdateContactsAsync(List<Contact> addingContacts)
        {
            _isContactsChangedEventEnabled = false;

            foreach (Contact addingContact in addingContacts)
                await AddOrUpdateContactAsync(addingContact).ConfigureAwait(false);

            _isContactsChangedEventEnabled = true;
            TriggerContactsChanged();
        }

        public async Task AnnounceContactIfNotAnnouncedYetAsync(string phoneNumber)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            phoneNumber = phoneNumber.ToE164PhoneNumberFormat();

            Contact announcingContact = Contacts.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            if (announcingContact == null || announcingContact.IsAnnouncedToService)
                return;

            if (announcingContact.IsBlocked)
                await DoctorServiceCommunicator.SendBlockedPhoneNumbersAsync(new string[] { phoneNumber })
                    .ConfigureAwait(false);
            else
                await DoctorServiceCommunicator.SendAllowedPhoneNumbersAsync(new string[] { phoneNumber })
                    .ConfigureAwait(false);

            //TODO: vahid: this is a wrong place to set the contact's IsAnnouncedToService property:
            //what if SendBlockedPhoneNumbersAsync and SendAllowedPhoneNumbersAsync weren't successful at sending
            //the number to server, say server was down at the time, or whatever reason.
            //We need refactor some codes such as make those two aforementioned methods to return a boolean indicating
            //whether they were successful or not.
            announcingContact.IsAnnouncedToService = true;
        }

        public async Task RemoveContactAsync(string phoneNumber)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            Contact contact = Contacts.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            if (contact == null)
                return;

            InternalContacts.Remove(contact);

            try
            {
                if (contact.IsBlocked)
                    await DoctorServiceCommunicator.RemoveBlockedPhoneNumbersAsync(new string[] { phoneNumber })
                        .ConfigureAwait(false);
                else
                    await DoctorServiceCommunicator.RemoveAllowedPhoneNumbersAsync(new string[] { phoneNumber })
                        .ConfigureAwait(true);
            }
            catch (ServiceCommunicationException)
            {
                // ignored
            }

            TriggerContactsChanged();
        }

        public async Task SynchronizeContactsAsync()
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            // Synchronizing Not Resa Phone Numbers
            var removedBlockedPhoneNumbers = new List<string>();
            var removedAllowedPhoneNumbers = new List<string>();
            var addedBlockedContacts = new List<Contact>();
            var addedAllowedContacts = new List<Contact>();
            var receivedBlockedPhoneNumbers =
                await DoctorServiceCommunicator.GetBlockedPhoneNumbersAsync().ConfigureAwait(false);
            var receivedAllowedPhoneNumbers =
                await DoctorServiceCommunicator.GetAllowedPhoneNumbersAsync().ConfigureAwait(false);

            //prepare two separate lists, one the blocked numbers that are in app but not in server,
            //and the other the allowed numbers that are in app but not in server, so that we add both lists numbers to server
            Contacts.Where(contact => !contact.IsResaContact).ToList().ForEach(contact =>
            {
                if (contact.IsBlocked && !receivedBlockedPhoneNumbers.Contains(contact.PhoneNumber))
                    addedBlockedContacts.Add(contact);
                else if (!contact.IsBlocked && !receivedAllowedPhoneNumbers.Contains(contact.PhoneNumber))
                    addedAllowedContacts.Add(contact);
            });

            //choose those blocked numbers which are in server but not in app so that we remove them later on
            receivedBlockedPhoneNumbers.ToList().ForEach(phoneNumber =>
            {
                if (!Contacts.Any(c => c.PhoneNumber == phoneNumber && c.IsBlocked))
                    removedBlockedPhoneNumbers.Add(phoneNumber);
            });

            //choose those allowed numbers which are in server but not in app so that we remove them later on
            receivedAllowedPhoneNumbers.ToList().ForEach(phoneNumber =>
            {
                if (!Contacts.Any(c => c.PhoneNumber == phoneNumber && !c.IsBlocked))
                    removedAllowedPhoneNumbers.Add(phoneNumber);
            });

            // TODO: Use better approach for gain performance
            Task[] tasks =
            {
                DoctorServiceCommunicator.SendAllowedPhoneNumbersAsync(addedAllowedContacts.Select(c => c.PhoneNumber)
                    .ToArray()),
                DoctorServiceCommunicator.SendBlockedPhoneNumbersAsync(addedBlockedContacts.Select(c => c.PhoneNumber)
                    .ToArray()),
                DoctorServiceCommunicator.RemoveAllowedPhoneNumbersAsync(removedAllowedPhoneNumbers.ToArray()),
                DoctorServiceCommunicator.RemoveBlockedPhoneNumbersAsync(removedBlockedPhoneNumbers.ToArray())
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var announcedCalls = new List<Contact>(
                addedBlockedContacts
                    .Concat(addedAllowedContacts)
                    .Select(contact =>
                    {
                        contact.IsAnnouncedToService = true;
                        return contact;
                    })
            );
            announcedCalls.ForEach(async contact => await AddOrUpdateContactAsync(contact).ConfigureAwait(false));

            // Synchronizing Resa Phone Numbers
            string[] resaPhoneNumbers =
                await DoctorServiceCommunicator.GetResaPhoneNumbersAsync().ConfigureAwait(false);
            resaPhoneNumbers
                .Select(Contact.Resa)
                .ToList()
                .ForEach(async contact =>
                    await AddOrUpdateContactAsync(contact)
                        .ConfigureAwait(false)
                );
        }

        public async Task SetStateAsync(Commons.DoctorState state)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            DoctorServiceCommunicator.OnSendDoctorStateResult += DoctorServiceCommunicatorOnOnSendDoctorStateResult;

            await DoctorServiceCommunicator.SendDoctorStateAsync(state).ConfigureAwait(false);

        }

        public async Task<Commons.DoctorState> GetStateAsync(CancellationToken cancellationToken = default)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            Commons.DoctorState state = await DoctorServiceCommunicator.GetDoctorStateAsync(cancellationToken).ConfigureAwait(false);

            State = state;

            return state;

        }

        /// <summary>
        /// <exception cref="ServiceCommunicationException">UserMustBeLoggedInException</exception>
        /// <exception cref="UserMustBeLoggedInException">UserMustBeLoggedInException</exception>
        /// <exception cref="AuthenticationException">UserMustBeLoggedInException</exception>
        /// <exception cref="ResaJsonException">UserMustBeLoggedInException</exception>
        /// <exception cref="NetworkConnectionException">UserMustBeLoggedInException</exception>
        /// <exception cref="AuthenticationException">UserMustBeLoggedInException</exception>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ICollection<CallbackRequest>> GetAllCallbackRequestsAsync(int offset = 0, int limit = int.MaxValue)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            var result = await DoctorServiceCommunicator.GetAllCallbackRequestsAsync(offset, limit).ConfigureAwait(false);

            if (result == null)
                return null;

            var callbacks = DoctorAppAutoMapper.Instance.Map<CallbackRequest[]>(result);
            return callbacks;
        }

        public async Task<ICollection<CallbackRequest>> GetActiveCallbackRequestsAsync(int offset = 0, int limit = int.MaxValue)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            var result = await DoctorServiceCommunicator.GetActiveCallbackRequestsAsync(offset, limit).ConfigureAwait(false);

            if (result == null)
                return null;

            return DoctorAppAutoMapper.Instance.Map<CallbackRequest[]>(result);
        }

        public async Task<string> GetCallbackRequestPhoneNumber(CancellationToken cancellationToken = default)
        {
            SetServiceCommunicatorToken();

            return await DoctorServiceCommunicator.GetCallbackRequestPhoneNumber(cancellationToken);
        }

        public async Task<ICollection<MedicalTest>> GetAllActiveMedicalTests(CancellationToken cancellationToken = default)
        {
            CheckLogin();

            SetServiceCommunicatorToken();

            var result = await DoctorServiceCommunicator.GetAllActiveMedicalTests(cancellationToken: cancellationToken).ConfigureAwait(false);

            if (result == null)
                return null;

            var callbacks = DoctorAppAutoMapper.Instance.Map<MedicalTest[]>(result);
            return callbacks;
        }

        protected void CheckLogin()
        {
            if (!IsLoggedIn)
                throw new UserMustBeLoggedInException();
        }

        private async Task UpdateAsync(bool checkLogin)
        {
            if (checkLogin)
                CheckLogin();

            SetServiceCommunicatorToken();

            DoctorPreviewViewModel doctorPreview = await DoctorServiceCommunicator.GetDoctorPreviewAsync().ConfigureAwait(false);
            DoctorAppAutoMapper.Instance.Map(doctorPreview, this);
        }

        private void TriggerContactsChanged()
        {
            if (_isContactsChangedEventEnabled)
                OnContactsChanged?.Invoke(Contacts);
        }

        private void SetServiceCommunicatorToken()
        {
            DoctorServiceCommunicator.CommunicationToken = DoctorServiceCommunicationToken.Create(ServiceCommuncationToken, Vsin);
        }

        private void DoctorServiceCommunicatorOnOnSendDoctorStateResult(Commons.DoctorState state, bool isSuccessful)
        {
            if (isSuccessful)
            {
                State = state;
            }

            OnSendDoctorStateResult?.Invoke(state, isSuccessful);

            DoctorServiceCommunicator.OnSendDoctorStateResult -= DoctorServiceCommunicatorOnOnSendDoctorStateResult;

        }

        #endregion Methods

        #region Private Fields

        private bool _isContactsChangedEventEnabled;

        #endregion Private Fields
    }
}