using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BSN.Resa.DoctorApp.Data.Repository
{
	public interface IDoctorRepository: ISingleRecordRepository<Doctor>
	{
		Doctor Get(bool includeContacts);
	}

	public class DoctorRepository: SingleRecordRepositoryBase<Doctor>, IDoctorRepository
	{
		public DoctorRepository(
			IDatabaseFactory databaseFactory, 
			IApplicationServiceCommunicator applicationServiceCommunicator, 
			IDoctorServiceCommunicator doctorServiceCommunicator)
			: base(databaseFactory)
		{
			_applicationServiceCommunicator = applicationServiceCommunicator;
			_doctorServiceCommunicator = doctorServiceCommunicator;
		}

		public Doctor Get(bool includeContacts)
		{
			if (includeContacts)
				Entity = DbSet.Include(Doctor.InternalORMPropertyAccessExpressions.InternalContacts).FirstOrDefaultAsync().Result;
			else
				Entity = DbSet.FirstOrDefaultAsync().Result;

			FillNotPersistedPropertiesOfEntity();

			return Entity;
		}

		public override Doctor Get()
		{
			return Get(includeContacts: true);
		}

		protected override void EnsureRecordExistance()
		{
			if (!DbSet.AnyAsync().Result)
			{
				Entity = new Doctor(_applicationServiceCommunicator, _doctorServiceCommunicator);
				DbSet.Add(Entity);
				Context.SaveChanges();
			}
		}

		protected override void FillNotPersistedPropertiesOfEntity()
		{
			Entity.ApplicationServiceCommunicator = _applicationServiceCommunicator;
			Entity.DoctorServiceCommunicator = _doctorServiceCommunicator;
		}

		private readonly IApplicationServiceCommunicator _applicationServiceCommunicator;

		private readonly IDoctorServiceCommunicator _doctorServiceCommunicator;
	}
}
