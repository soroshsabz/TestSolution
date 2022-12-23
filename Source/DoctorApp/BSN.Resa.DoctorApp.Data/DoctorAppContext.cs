using BSN.Resa.DoctorApp.Data.Configurations;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BSN.Resa.DoctorApp.Data
{
	public class DoctorAppContext: DbContext
    {
		// Just for ORM use
		public DoctorAppContext() { }

		public DoctorAppContext(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
            Database.Migrate();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(_dbConnection?.ConnectionString ?? "Data Source=BSN.Resa.DoctorApp.db");
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.ApplyConfiguration(new DoctorConfiguration());
			builder.ApplyConfiguration(new ContactConfiguration());
			builder.ApplyConfiguration(new AppUpdateConfiguration());
		    builder.ApplyConfiguration(new CallbackRequestConfiguration());
		    builder.ApplyConfiguration(new MedicalTestConfiguration());
        }

		public DbSet<AppUpdate> AppUpdates { get; set; }

		public DbSet<Doctor> Doctors { get; set; }

		public DbSet<Contact> Contacts { get; set; }

        public DbSet<CallbackRequest> CallbackRequests { get; set; }

        public DbSet<MedicalTest> MedicalTests { get; set; }

        private readonly IDbConnection _dbConnection;
    }
}
