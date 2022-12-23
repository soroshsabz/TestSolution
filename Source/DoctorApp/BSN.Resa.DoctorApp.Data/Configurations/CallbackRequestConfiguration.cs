using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSN.Resa.DoctorApp.Data.Configurations
{
    public class CallbackRequestConfiguration : IEntityTypeConfiguration<CallbackRequest>
    {
        public void Configure(EntityTypeBuilder<CallbackRequest> builder)
        {
            builder.HasKey(property => property.Id);
        }
    }
}
