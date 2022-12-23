using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;

namespace BSN.Resa.DoctorApp.Droid
{
    public class ConfigAndroid: ConfigAndroidBase
    {
        public override MobilePlatform TargetPlatform => MobilePlatform.Android;

		public override string ResaMobileApiAddress => "https://resaa.net/api/Mobile/";

		public override string ResaDoctorAppApiAddress => "https://resaa.net/api/DoctorApp/";

		public override string ResaSmsReceiverPhoneNumber => "10008000888808";

		public override string ResaMobileApiToken => "5991848b-87ce-4646-a332-b7f2c7cfb221";

		public override string AppCenterAppSecret => "bd4ce325-f529-4426-a18f-e751033e8a72";

        public override string ResaPrivacyPolicyUrl => "https://resaa.net/privacy";

		#region Singleton

		public static IConfig Instance => _instance ?? (_instance = new ConfigAndroid());

		private static IConfig _instance;

		#endregion
	}
}