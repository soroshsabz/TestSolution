using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Data.Configurations
{
    public class MedicalTestConfiguration : IEntityTypeConfiguration<MedicalTest>
    {
        public void Configure(EntityTypeBuilder<MedicalTest> builder)
        {
            builder.HasKey(property => property.Id);
            builder.Property(medicalTest => medicalTest.Photos).HasConversion(
                value => JsonConvert.SerializeObject(value),
                value => JsonConvert.DeserializeObject<List<string>>(value)
            );

            builder.Ignore(medicalTest => medicalTest.VoiceFileStream);
            builder.Ignore(medicalTest => medicalTest.TextReply);
        }
    }
}