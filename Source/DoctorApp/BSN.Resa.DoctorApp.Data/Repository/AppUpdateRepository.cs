using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BSN.Resa.DoctorApp.Data.Repository
{
	public interface IAppUpdateRepository : ISingleRecordRepository<AppUpdate>
	{ }

	public class AppUpdateRepository : SingleRecordRepositoryBase<AppUpdate>, IAppUpdateRepository
	{
		public AppUpdateRepository(
			IDatabaseFactory databaseFactory, 
			IApplicationServiceCommunicator applicationServiceCommunicator)
			: base(databaseFactory)
		{
			_applicationServiceCommunicator = applicationServiceCommunicator;
		}

		protected override void EnsureRecordExistance()
		{
			if (!DbSet.AnyAsync().Result)
			{
				Entity = new AppUpdate(_applicationServiceCommunicator);
				DbSet.Add(Entity);
				Context.SaveChanges();
			}
		}

		protected override void FillNotPersistedPropertiesOfEntity()
		{
			Entity.ApplicationServiceCommunicator = _applicationServiceCommunicator;
		}

		private IApplicationServiceCommunicator _applicationServiceCommunicator;
	}
}