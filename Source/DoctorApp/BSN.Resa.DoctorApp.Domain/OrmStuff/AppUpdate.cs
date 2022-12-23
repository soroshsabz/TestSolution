using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BSN.Resa.DoctorApp.Data")]

namespace BSN.Resa.DoctorApp.Domain.Models
{
	public partial class AppUpdate
	{
		internal protected AppUpdate()
		{}

		private int Id { get; set; }

		private string LatestDownloadableAppUpdateVersionLocallyInString { get; set; }

		internal static class InternalORMPropertyAccessExpressions
		{
			public static readonly Expression<Func<AppUpdate, object>> Id = x => x.Id;

			public static readonly Expression<Func<AppUpdate, object>> LatestDownloadableAppUpdateVersionLocallyInString = x => x.LatestDownloadableAppUpdateVersionLocallyInString;
		}
	}
}