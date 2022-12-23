using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSN.Resa.DoctorApp.Data.Configurations
{
	public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
	{
		public void Configure(EntityTypeBuilder<Doctor> builder)
		{
			builder.HasKey(Doctor.InternalORMPropertyAccessExpressions.Id);

			builder.HasMany(Doctor.InternalORMPropertyAccessExpressions.InternalContacts)
				   .WithOne()
				   .IsRequired();

			builder.OwnsOne(Doctor.InternalORMPropertyAccessExpressions.ServiceCommunicationToken);

			builder.Ignore(x => x.DoctorServiceCommunicator)
				   .Ignore(x => x.ApplicationServiceCommunicator)
				   .Ignore(x => x.Contacts);
		}
	}
}
