using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSN.Resa.DoctorApp.Data.Configurations
{
	public class ContactConfiguration : IEntityTypeConfiguration<Contact>
	{
		public void Configure(EntityTypeBuilder<Contact> builder)
		{
			builder.HasKey(Contact.InternalORMPropertyAccessExpressions.Id);

			builder.HasIndex(x => x.PhoneNumber).IsUnique();
		}
	}
}
