using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BSN.Resa.DoctorApp.Data")]

namespace BSN.Resa.DoctorApp.Domain.Models
{
	public partial class Doctor
    {
		internal Doctor()
		{
			InternalContacts = new List<Contact>();
			_isContactsChangedEventEnabled = true;
		}

		internal static class InternalORMPropertyAccessExpressions
		{
			public static readonly Expression<Func<Doctor, object>> Id = x => x.Id;

			public static readonly Expression<Func<Doctor, IEnumerable<Contact>>> InternalContacts = x => x.InternalContacts;

			public static readonly Expression<Func<Doctor, OauthToken>> ServiceCommunicationToken = x => x.ServiceCommuncationToken;
		}
	}
}