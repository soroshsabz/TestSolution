using BSN.Resa.DoctorApp.Data.Infrastructure;
using System;
using System.IO;

namespace BSN.Resa.DoctorApp.Droid.Infrastructure
{
    public class DbConnectionAndroid : IDbConnection
	{
		public string ConnectionString => $"Data Source={DatabaseFilePath}";

	    public string DatabaseFilePath
	    {
	        get
	        {
	            string dbFileName = "BSN.Resa.DoctorApp.db";

	            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

	            return Path.Combine(documentsPath, dbFileName);
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