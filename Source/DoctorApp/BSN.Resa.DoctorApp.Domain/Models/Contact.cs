using BSN.Resa.DoctorApp.Commons.Utilities;
using System;
using BSN.Resa.DoctorApp.Domain.Utilities;

namespace BSN.Resa.DoctorApp.Domain.Models
{
	public class CannotEditResaPhoneNumbersException : Exception
	{ }

	public partial class Contact
    {
		private Contact(string phoneNumber)
		{
			PhoneNumber = phoneNumber.ToE164PhoneNumberFormat();
			IsBlocked = false;
			IsVisible = true;
			IsResaContact = false;
			IsAnnouncedToService = false;
			BlockedCount = 0;
		}

		public string PhoneNumber { get; protected set; }

		public bool IsBlocked { get; protected set; }

		public bool IsVisible { get; protected set; }

		public bool IsResaContact { get; protected set; }

		public bool IsAnnouncedToService { get; set; }

		public int BlockedCount { get; protected set; }

		public void Update(Contact contact)
		{
			if (IsResaContact != contact.IsResaContact)
				throw new CannotEditResaPhoneNumbersException();

			DoctorAppAutoMapper.Instance.Map(contact, this);
		}

		public void IncreaseBlockedCount()
		{
			++BlockedCount;
		}

		public static Contact Resa(string phoneNumber)
		{
			return new Contact(phoneNumber)
			{
				IsBlocked = false,
				IsVisible = false,
				IsResaContact = true,
				IsAnnouncedToService = true
			};
		}

		public static Contact Allowed(string phoneNumber, bool isSynchronized = false, bool isVisible = true)
		{
			return new Contact(phoneNumber)
			{
				IsBlocked = false,
				IsAnnouncedToService = isSynchronized,
				IsVisible = isVisible
			};
		}

		public static Contact Blocked(string phoneNumber, bool isSynchronized = false, bool isVisible = true)
		{
			return new Contact(phoneNumber)
			{
				IsBlocked = true,
				IsAnnouncedToService = isSynchronized,
				IsVisible = isVisible
			};
		}
	}
}
