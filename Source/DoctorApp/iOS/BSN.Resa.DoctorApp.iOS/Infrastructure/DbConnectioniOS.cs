using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.iOS.Commons;
using Foundation;
using System;
using System.IO;


namespace BSN.Resa.DoctorApp.iOS.Infrastructure
{
    public class DbConnectioniOS : IDbConnection
    {
        private const string Filename = "BSN.Resa.DoctorApp.db";

        public string ConnectionString => $"Data Source={DatabaseFilePath}";

        public string DatabaseFilePath
        {
            get
            {
                string groupPath = NSFileManager.DefaultManager.GetContainerUrl(ConfigiOS.Instance.AppGroupIdentifier)?.Path;

                if (groupPath == null)
                    throw new Exception($"App group \"{ConfigiOS.Instance.AppGroupIdentifier}\" doesn't exist.");

                return Path.Combine(groupPath, Filename);
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