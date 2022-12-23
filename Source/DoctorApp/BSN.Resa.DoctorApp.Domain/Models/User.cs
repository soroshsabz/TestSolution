using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Domain.Utilities;

namespace BSN.Resa.DoctorApp.Domain.Models
{
	public abstract partial class User
	{
		#region Properties

		public bool IsLoggedIn { get; protected set; }

		protected OauthToken ServiceCommuncationToken { get; set; }

		#endregion

		public async Task<OauthToken> AuthenticateAsync(string username, string password)
		{
			var result = await ApplicationServiceCommunicator.AuthenticateAsync(username, password).ConfigureAwait(false);
		    DoctorAppAutoMapper.Instance.Map(result, ServiceCommuncationToken);
            return result;
		}

		public IApplicationServiceCommunicator ApplicationServiceCommunicator { get; set; }
    }
}
