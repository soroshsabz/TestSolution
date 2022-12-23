using BSN.Resa.DoctorApp.EventConsumers.InternetAccessedConsumers;
using BSN.Resa.DoctorApp.iOS.Commons;
using BSN.Resa.DoctorApp.iOS.Commons.Utilities;
using BSN.Resa.DoctorApp.iOS.EventConsumers.AppStartConsumers;
using BSN.Resa.DoctorApp.iOS.EventConsumers.ContactsChangedConsumers;
using BSN.Resa.DoctorApp.iOS.EventConsumers.UrlCallConsumers;
using Unity;
using static BSN.Resa.DoctorApp.Utilities.DependencyInjectionHelper;

namespace BSN.Resa.DoctorApp.iOS
{
    public partial class AppDelegate
	{
		private void RegisterDependencies()
		{
			Container.RegisterInstance(BlockedPhoneNumbers.Instance);
            Container.RegisterType<IConfigiOS, ConfigiOS>();
            Container.RegisterType<IContactsChangedConsumer, ContactsAndBlockedPhoneNumbersSynchronizer>();
			Container.RegisterType<IUrlCallConsumer, ShareContactUrlCallConsumer>();
			Container.RegisterType<IAppStartConsumer, AppStartConsumer>();
			Container.RegisterType<IInternetAccessedConsumer, PhoneNumbersSynchronizer>();
		}
	}
}