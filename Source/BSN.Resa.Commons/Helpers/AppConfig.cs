using System.Configuration;
using BSN.Resa.Core.Commons;

namespace BSN.Resa.Commons.Helpers
{
    public class AppConfig : IAppConfig
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
