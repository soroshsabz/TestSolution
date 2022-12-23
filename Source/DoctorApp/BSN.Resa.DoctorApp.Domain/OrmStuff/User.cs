using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BSN.Resa.DoctorApp.Data")]

namespace BSN.Resa.DoctorApp.Domain.Models
{
	public abstract partial class User
    {
		internal User()
		{
			ServiceCommuncationToken = new Commons.ServiceCommunicators.OauthToken();
		}

		protected int Id { get; set; }
	}
}