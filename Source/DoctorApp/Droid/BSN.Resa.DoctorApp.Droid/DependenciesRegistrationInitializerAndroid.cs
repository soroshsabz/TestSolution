using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Services;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Droid.DeviceManipulators;
using BSN.Resa.DoctorApp.Droid.Infrastructure;
using BSN.Resa.DoctorApp.Droid.Services;
using BSN.Resa.DoctorApp.Droid.Utilities;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using Prism;
using Prism.Ioc;

namespace BSN.Resa.DoctorApp.Droid
{
    public class DependenciesRegistrationInitializerAndroid : IPlatformInitializer
    {
        /// <summary>
        /// Here is the centralized place to register all Android-specific dependencies
        /// For official doc <a href="https://prismlibrary.github.io/docs/xamarin-forms/dependency-injection/platform-specific-services.html">visit here</a>
        /// </summary>
        /// <param name="containerRegistry"></param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IGsmConnection, GsmConnectionAndroid>();
            containerRegistry.RegisterSingleton<IResaPermissions, ResaPermissions>();
            containerRegistry.RegisterSingleton<IMobileContactManager, MobileContactManagerAndroid>();
            containerRegistry.RegisterSingleton<IConfig, ConfigAndroid>();
            containerRegistry.RegisterSingleton<ICallBlockAndIdentification, CallBlockAndIdentificationAndroid>();
            containerRegistry.RegisterSingleton<IApplicationManipulator, ApplicationManipulatorAndroid>();
            containerRegistry.RegisterSingleton<IAppUpdateHelper, AppUpdateHelperAndroid>();
            containerRegistry.RegisterSingleton<IDeviceInfo, DeviceInfoAndroid>();
            containerRegistry.RegisterSingleton<IResaService, ResaService>();
            containerRegistry.RegisterSingleton<IDbConnection, DbConnectionAndroid>();
            containerRegistry.RegisterSingleton<IPhotoViewer, PhotoViewer>();
            containerRegistry.RegisterSingleton<IVoiceRecorder, VoiceRecorder>();
            containerRegistry.RegisterSingleton<INativeHttpMessageHandlerProvider, HttpMessageHandlerProviderAndroid>();
        }
    }
}