using BSN.Resa.DoctorApp.Data.Infrastructure;
using System;
using System.IO;


namespace BSN.Resa.DoctorApp.iOS.LocalMarkets.Infrastructure
{
    public class DbConnectioniOS : IDbConnection
	{
		private const string Filename = "BSN.Resa.DoctorApp.db";

		public string ConnectionString => $"Data Source={DatabaseFilePath}";

	    public string DatabaseFilePath
	    {
	        get
	        {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                return Path.Combine(documentsPath, "..", "Library", Filename);
            }
	    }

        public bool DeleteDatabaseFile()
        {
            try
            {
                File.Delete(DatabaseFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}