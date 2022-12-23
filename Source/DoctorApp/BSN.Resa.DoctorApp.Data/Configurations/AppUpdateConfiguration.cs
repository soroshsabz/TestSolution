using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSN.Resa.DoctorApp.Data.Configurations
{
	public class AppUpdateConfiguration : IEntityTypeConfiguration<AppUpdate>
	{
		public void Configure(EntityTypeBuilder<AppUpdate> builder)
		{
			builder.HasKey(AppUpdate.InternalORMPropertyAccessExpressions.Id);

			builder.Property(AppUpdate.InternalORMPropertyAccessExpressions.LatestDownloadableAppUpdateVersionLocallyInString);

			builder.Ignore(x => x.LatestDownloadableAppUpdateVersionLocally)
				   .Ignore(x => x.ApplicationServiceCommunicator);
		}
	}
}