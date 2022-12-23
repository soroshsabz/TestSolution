using BSN.Resa.DoctorApp.Commons;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets
{
    public class Config : ConfigBase
    {
        public override string AppCenterAppSecret => "93b9954d-7961-472c-acf6-f1810ecbdfa3";

        #region Singleton

        public static IConfig Instance => _instance ?? (_instance = new Config());

        private static IConfig _instance;

        #endregion
    }
}