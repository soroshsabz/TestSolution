namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
	public class DoctorServiceCommunicationToken
	{
		private DoctorServiceCommunicationToken() {}

		public string Vsin { get; private set; }

		public OauthToken OauthToken { get; private set; }

		public static DoctorServiceCommunicationToken Create(string vsin)
		{
			return Create(null, vsin);
		}

		public static DoctorServiceCommunicationToken Create(OauthToken oauthToken)
		{
			return Create(oauthToken, null);
		}
		
		public static DoctorServiceCommunicationToken Create(OauthToken oauthToken, string vsin)
		{
			return new DoctorServiceCommunicationToken
			{
				Vsin = vsin,
				OauthToken = oauthToken
			};
		}
    }
}
