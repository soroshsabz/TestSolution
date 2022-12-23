using AddressBook;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Services;
using Contacts;
using Foundation;
using System;

namespace BSN.Resa.DoctorApp.iOS.DeviceManipulators
{
    public class MobileContactManageriOS : IMobileContactManager
    {
        public MobileContactManageriOS(IApplicationStatistics applicationStatistics)
        {
            _applicationStatistics = applicationStatistics;
        }

        // Visit https://stackoverflow.com/questions/4821661/how-do-you-add-contacts-to-the-iphone-address-book-with-monotouch
        public void AddContact(string firstName, string lastName, string[] phoneNumbers, string imageFileName)
        {
            if (ABAddressBook.GetAuthorizationStatus() == ABAuthorizationStatus.Denied)
                throw new UnauthorizedAccessException();

            var addressBook = new ABAddressBook();
            addressBook.RequestAccess((isAllowed, error) =>
            {
                if (isAllowed)
                {
                    CNContact previousContact = GetContactByName(firstName, lastName);

                    if (previousContact != null)
                    {
                        RemoveContact(previousContact);
                        _applicationStatistics.SendEvent(
                            $"Previous contact {firstName} {lastName} with phone numbers: {previousContact.PhoneNumbers} is removed!");
                    }

                    addressBook.Add(CreatePerson(firstName, lastName, phoneNumbers, imageFileName));
                    addressBook.Save();
                }
                else
                {
                    throw new UnauthorizedAccessException(error.ToString());
                }
            });
        }

        private static ABPerson CreatePerson(string firstName, string lastName, string[] phoneNumbers,
            string imageFileName)
        {
            var person = new ABPerson
            {
                FirstName = firstName,
                LastName = lastName
            };

            ABMutableMultiValue<string> phones = new ABMutableStringMultiValue();
            foreach (string phoneNumber in phoneNumbers)
                phones.Add(phoneNumber, ABPersonPhoneLabel.Main);
            person.SetPhones(phones);

            person.Image = NSData.FromFile(imageFileName);

            return person;
        }

        // Visit https://forums.xamarin.com/discussion/89860/how-do-you-get-all-contacts
        private static CNContact GetContactByName(string firstName, string lastName)
        {
            var keysToFetch = new[] {CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers};
            var containerId = new CNContactStore().DefaultContainerIdentifier;
            NSError error;
            CNContact[] contacts;
            using (var predicate = CNContact.GetPredicateForContactsInContainer(containerId))
            {
                using (var store = new CNContactStore())
                {
                    contacts = store.GetUnifiedContacts(predicate, keysToFetch, out error);
                }
            }

            if (error != null)
                throw new Exception(error.ToString());

            foreach (CNContact contact in contacts)
                if (contact.GivenName == firstName && contact.FamilyName == lastName)
                    return contact;

            return null;
        }

        // Visit https://stackoverflow.com/questions/37225374/programmatically-remove-contact-from-adress-book-on-specific-time-in-swift
        private static void RemoveContact(CNContact contact)
        {
            var request = new CNSaveRequest();
            request.DeleteContact(contact.MutableCopy() as CNMutableContact);
            NSError error;
            using (var store = new CNContactStore())
            {
                store.ExecuteSaveRequest(request, out error);
            }

            if (error != null)
                throw new Exception(error.ToString());
        }

        public bool Contains(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        private readonly IApplicationStatistics _applicationStatistics;
    }
}