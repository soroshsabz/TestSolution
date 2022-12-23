namespace BSN.Resa.DoctorApp.iOS.Commons
{
	public class ConfigiOS: ConfigiOSBase
    {
        public override string ResaSmsReceiverPhoneNumber => "10008000888808";

		public override string AppCenterAppSecret => "093fb647-121a-478a-ad6e-f373cc0d8aab";

		#region Singleton

		public static IConfigiOS Instance => _instance ?? (_instance = new ConfigiOS());

		private static IConfigiOS _instance;

		#endregion
	}
}
