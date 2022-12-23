namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
	public class OauthToken
    {
		public string AccessToken { get; set; }

		public string TokenType { get; set; }

		public string ExpiresIn { get; set; }

		public string Authorization => $"{TokenType} {AccessToken}";
	}
}
