using BSN.Resa.DoctorApp.iOS.Commons;
using Unity;
using static BSN.Resa.DoctorApp.Utilities.DependencyInjectionHelper;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets
{
    public partial class AppDelegate
	{
		private void RegisterDependencies()
		{
            Container.RegisterType<IConfigiOS, ConfigiOS>();
		}
	}
}