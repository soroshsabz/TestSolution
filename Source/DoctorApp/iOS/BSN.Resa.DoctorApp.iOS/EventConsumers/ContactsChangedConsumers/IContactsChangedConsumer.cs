using System.Collections.Generic;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.EventConsumers;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.ContactsChangedConsumers
{
	public interface IContactsChangedConsumer : IEventConsumer
	{
		void OnContactsChanged(IReadOnlyCollection<Contact> contacts);
	}
}