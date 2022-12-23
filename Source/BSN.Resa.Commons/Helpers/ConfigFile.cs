using System;

namespace BSN.Resa.Commons.Helpers
{
    public class ConfigFile
    {
        private ConfigFile(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public int VsinLength => _appSettings.Get<int>("LengthOfVariableSubscriberNumber", 6);

        public int DoctorVsinLength => _appSettings.Get<int>("LengthOfVariableSubscriberNumberOfDoctor", 4);

        private readonly static Lazy<ConfigFile> _instance =
            new Lazy<ConfigFile>(() => new ConfigFile(AppSettings.GetInstance()), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly AppSettings _appSettings;

        public static ConfigFile Instance => _instance.Value;
    }
}