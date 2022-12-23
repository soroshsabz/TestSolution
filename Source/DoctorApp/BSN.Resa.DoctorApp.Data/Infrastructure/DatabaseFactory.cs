namespace BSN.Resa.DoctorApp.Data.Infrastructure
{
	public interface IDatabaseFactory
	{
		DoctorAppContext Context { get; }
	}

	public class DatabaseFactory: IDatabaseFactory
	{
		public DoctorAppContext Context => _context ?? (_context = new DoctorAppContext(_dbConnection));

		public DatabaseFactory(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		private DoctorAppContext _context;

		private readonly IDbConnection _dbConnection;
	}
}
