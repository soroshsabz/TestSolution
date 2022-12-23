using System.Collections.Generic;
using System.Linq;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.iOS.Commons.Utilities;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.ContactsChangedConsumers
{
	public class ContactsAndBlockedPhoneNumbersSynchronizer : IContactsChangedConsumer
	{
		public ContactsAndBlockedPhoneNumbersSynchronizer(
			IBlockedPhoneNumbers blockedPhoneNumbers,
			ICallBlockAndIdentification callBlockAndIdentification)
		{
			_blockedPhoneNumbers = blockedPhoneNumbers;
			_callBlockAndIdentification = callBlockAndIdentification;
		}

		public void OnContactsChanged(IReadOnlyCollection<Contact> contacts)
		{
			_blockedPhoneNumbers.SetBlockedPhoneNumbers(
				contacts.Where(c => c.IsBlocked)
				.Select(x => long.Parse(x.PhoneNumber))
				.ToList());

			_callBlockAndIdentification.RefreshBlockedPhoneNumbers();
		}

		private readonly IBlockedPhoneNumbers _blockedPhoneNumbers;

		private readonly ICallBlockAndIdentification _callBlockAndIdentification;
	}
}