using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace BSN.Resa.DoctorApp.Utilities
{
    public class AppCenterInitializer
    {
        public static void Init(IConfig config)
        {
            AppCenter.Start(config.AppCenterAppSecret, typeof(Analytics), typeof(Crashes));

            var userId = DoctorAppSettings.AppCenterUserId.IsNullOrEmptyOrSpace() ?
                "User not Logged In" : DoctorAppSettings.AppCenterUserId;

            AppCenter.SetUserId(userId);
        }
    }
}
