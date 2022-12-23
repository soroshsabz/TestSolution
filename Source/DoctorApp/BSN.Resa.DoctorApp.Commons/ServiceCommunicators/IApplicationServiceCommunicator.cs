using BSN.Resa.Core.Commons.ViewModels;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
	public interface IApplicationServiceCommunicator
    {
		Task<OauthToken> AuthenticateAsync(string username, string password);

		Task<IServiceResult<AppUpdateManifest>> GetAppUpdateManifestAsync();
	}
}
