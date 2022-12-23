using BSN.Resa.DoctorApp.Commons;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets
{
    public class Config : ConfigBase
    {
        public override string AppCenterAppSecret => "1d5619f8-38eb-486b-a4ee-9cb682b43095";

        #region Singleton

        public static IConfig Instance => _instance ?? (_instance = new Config());

        private static IConfig _instance;

        #endregion
    }
}