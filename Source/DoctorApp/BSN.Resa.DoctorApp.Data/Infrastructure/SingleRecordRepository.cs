using Microsoft.EntityFrameworkCore;

namespace BSN.Resa.DoctorApp.Data.Infrastructure
{
	public interface ISingleRecordRepository<TEntity> where TEntity : class
	{
		TEntity Get();

		void Update();
	}

	public abstract class SingleRecordRepositoryBase<TEntity>: ISingleRecordRepository<TEntity>
		where TEntity: class
	{
		public SingleRecordRepositoryBase(IDatabaseFactory databaseFactory)
		{
			DbSet = databaseFactory.Context.Set<TEntity>();
			Context = databaseFactory.Context;
			
			EnsureRecordExistance();
		}

		public virtual TEntity Get()
		{
			Entity = DbSet.FirstOrDefaultAsync().Result;

			FillNotPersistedPropertiesOfEntity();

			return Entity;
		}

		public void Update()
		{
			DbSet.Update(Entity);
		}

		protected abstract void EnsureRecordExistance();

		protected abstract void FillNotPersistedPropertiesOfEntity();

		protected TEntity Entity;

		protected readonly DoctorAppContext Context;

		protected readonly DbSet<TEntity> DbSet;
	}
}