namespace BSN.Resa.DoctorApp.Data.Infrastructure
{
	public interface IDbConnection
    {
		string ConnectionString { get; }

        string DatabaseFilePath { get; }

        bool DeleteDatabaseFile();
    }
}
