namespace BSN.Resa.DoctorApp.iOS.Commons
{
    public class ConfigiOS: ConfigiOSBase
    {
        public override string ResaSmsReceiverPhoneNumber => "10008000888808";

		public override string AppCenterAppSecret => "b098d120-049e-47cd-8378-360e5bf1d676";

		#region Singleton

		public static IConfigiOS Instance => _instance ?? (_instance = new ConfigiOS());

		private static IConfigiOS _instance;

		#endregion
	}
}