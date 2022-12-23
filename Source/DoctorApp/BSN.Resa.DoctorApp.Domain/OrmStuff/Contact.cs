using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BSN.Resa.DoctorApp.Data")]

namespace BSN.Resa.DoctorApp.Domain.Models
{
	public partial class Contact
    {
		internal Contact()
		{ }

		private long Id { get; set; }

		internal static class InternalORMPropertyAccessExpressions
		{
			public static readonly Expression<Func<Contact, object>> Id = x => x.Id;
		}
	}
}
