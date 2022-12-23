namespace BSN.Resa.DoctorApp.Data.Infrastructure
{
	public interface IUnitOfWork
	{
		void Commit();
	}
    public class UnitOfWork: IUnitOfWork
    {
		public UnitOfWork(IDatabaseFactory databaseFactory)
		{
			_databaseFactory = databaseFactory;
		}

		public void Commit()
		{
			_databaseFactory.Context.SaveChanges();
		}

		private readonly IDatabaseFactory _databaseFactory;
    }
}
