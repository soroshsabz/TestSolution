using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.iOS.LocalMarkets.DeviceManipulators;
using BSN.Resa.DoctorApp.iOS.LocalMarkets.Infrastructure;
using BSN.Resa.DoctorApp.iOS.LocalMarkets.Services;
using BSN.Resa.DoctorApp.iOS.LocalMarkets.Utilities;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets
{
    public class DependenciesRegistrationInitializeriOS : IPlatformInitializer
    {
        /// <summary>
        /// Here is the centralized place to register all iOS-specific dependencies
        /// For official doc <a href="https://prismlibrary.github.io/docs/xamarin-forms/dependency-injection/platform-specific-services.html">visit here</a>
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IGsmConnection, GsmConnectioniOS>();
            containerRegistry.GetContainer().RegisterFactory<IResaPermissions>(container => null); //we didn't need to implement IResaPermissions in iOS, so we pass a null dependency
            containerRegistry.RegisterSingleton<IMobileContactManager, MobileContactManageriOS>();
            containerRegistry.RegisterSingleton<IConfig, Config>();
            containerRegistry.RegisterSingleton<IApplicationManipulator, ApplicationManipulatoriOS>();
            containerRegistry.RegisterSingleton<IAppUpdateHelper, AppUpdateHelperiOS>();
            containerRegistry.RegisterSingleton<IDeviceInfo, DeviceInfoiOS>();
            containerRegistry.RegisterSingleton<IResaService, ResaService>();
            containerRegistry.RegisterSingleton<IDbConnection, DbConnectioniOS>();
            containerRegistry.RegisterSingleton<IPhotoViewer, PhotoViewer>();
            containerRegistry.RegisterSingleton<IVoiceRecorder, VoiceRecorder>();
        }
    }
}