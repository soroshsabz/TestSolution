using Acr.UserDialogs;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators.ApplicationServiceCommunicator;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators.DoctorServiceCommunicator;
using BSN.Resa.DoctorApp.EventConsumers.PrismAppStartConsumers;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.DoctorApp.ViewModels;
using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using BSN.Resa.DoctorApp.ViewModels.Contacts;
using BSN.Resa.DoctorApp.ViewModels.MedicalTests;
using BSN.Resa.DoctorApp.Views;
using BSN.Resa.DoctorApp.Views.CallbackRequests;
using BSN.Resa.DoctorApp.Views.Contacts;
using BSN.Resa.DoctorApp.Views.Controls;
using BSN.Resa.DoctorApp.Views.MedicalTests;
using BSN.Resa.Locale;
using Plugin.Badge;
using Plugin.Connectivity.Abstractions;
using Plugin.LocalNotifications;
using Plugin.Messaging;
using Plugin.Permissions;
using Plugin.SimpleAudioPlayer;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using System.Globalization;
using Unity;
using Unity.Lifetime;
using Xamarin.Forms.Xaml;
using FlyoutPage = BSN.Resa.DoctorApp.Views.FlyoutPage;

namespace BSN.Resa.DoctorApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer)
        {
            DependencyInjectionHelper.Container = ((PrismApplicationBase)Xamarin.Forms.Application.Current).Container.GetContainer();
        }

        #region Public Static stuff

        public static CustomFlyoutPage CustomFlyoutPage { get; set; }
        public static App CurrentInstance;
        public static SupportedLanguages CurrentLanguage;

        #endregion

        protected override async void OnInitialized()
        {
            SetLocale();

            InitializeComponent();

            CurrentInstance = this;

            await NavigationService.NavigateAsync(Container.Resolve<IPrismAppStartConsumer>().OnStart());
        }

        protected override void RegisterTypes(IContainerRegistry registry)
        {
            RegisterPages(registry);

            RegisterDependencies(registry);
        }

        private static void RegisterPages(IContainerRegistry registry)
        {
#if DEBUG
            registry.RegisterForNavigation<TestPage, TestPageViewModel>();
#endif
            registry.RegisterForNavigation<FlyoutPage, NavigationDrawerViewModel>();
            registry.RegisterForNavigation<AppNavigationPage>();
            registry.RegisterForNavigation<MenuPage, NavigationDrawerViewModel>();
            registry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            registry.RegisterForNavigation<HelpPage, HelpPageViewModel>();
            registry.RegisterForNavigation<AboutPage, AboutPageViewModel>();
            registry.RegisterForNavigation<BlockedContactsListPage, BlockedContactsListPageViewModel>();
            registry.RegisterForNavigation<AllowedContactsListPage, AllowedContactsListPageViewModel>();
            registry.RegisterForNavigation<UrgentUpdatePage, UrgentUpdatePageViewModel>();
            registry.RegisterForNavigation<CallbackRequestsPage, CallbackRequestsPageViewModel>();
            registry.RegisterForNavigation<DoctorStatePage, DoctorStatePageViewModel>();
            registry.RegisterForNavigation<CallbackRequestsHistoryPage, CallbackRequestsHistoryPageViewModel>();
            registry.RegisterForNavigation<AppSettingsPage, AppSettingsPageViewModel>(); //it's only for android
            registry.RegisterForNavigation<AppSettingsAdvancedPage, AppSettingsAdvancedPageViewModel>(); //it's only for android
            registry.RegisterForNavigation<PermissionsPage, PermissionsPageViewModel>();
            registry.RegisterForNavigation<ActiveMedicalTestsPage, ActiveMedicalTestsPageViewModel>();
            registry.RegisterForNavigation<MedicalTestPage, MedicalTestPageViewModel>();
        }

        private void RegisterDependencies(IContainerRegistry registry)
        {
            /*
                Note: all platform-specific dependencies are registered in their own projects,
                for Android see the class DependenciesRegistrationInitializerAndroid,
                and for iOS see the class DependenciesRegistrationInitializeriOS.
                To know why we use this approach see the official doc:
                https://prismlibrary.github.io/docs/xamarin-forms/dependency-injection/platform-specific-services.html
            */

            registry.RegisterInstance(Plugin.Connectivity.CrossConnectivity.Current);
            registry.RegisterInstance(CrossMessaging.Current.SmsMessenger);
            registry.RegisterInstance(CrossMessaging.Current.PhoneDialer);
            registry.RegisterInstance(CrossBadge.Current);
            registry.RegisterInstance(CrossLocalNotifications.Current);
            registry.RegisterInstance(UserDialogs.Instance);
            registry.RegisterInstance(CrossPermissions.Current);
            registry.RegisterSingleton<IPermissionsManager, PermissionsManager>();
            registry.RegisterInstance(new ConnectionStatusManager());
            registry.RegisterSingleton<ICrashReporter, AppCenterCrashReporter>();
            registry.RegisterSingleton<IApplicationStatistics, AppCenterApplicationStatistics>();
            registry.RegisterSingleton<IApplicationServiceCommunicator, ApplicationServiceCommunicator>();
            registry.RegisterPerResolveLifeTime<IDatabaseFactory, DatabaseFactory>();
            registry.RegisterPerResolveLifeTime<IDoctorRepository, DoctorRepository>();
            registry.RegisterPerResolveLifeTime<IAppUpdateRepository, AppUpdateRepository>();
            registry.RegisterPerResolveLifeTime<IUnitOfWork, UnitOfWork>();
            registry.RegisterPerResolveLifeTime<ICallbackRequestRepository, CallbackRequestRepository>();
            registry.RegisterPerResolveLifeTime<IMedicalTestRepository, MedicalTestRepository>();
            registry.Register<IPrismAppStartConsumer, PrismAppStartConsumer>();
            registry.RegisterInstance(CrossSimpleAudioPlayer.Current);

            RegisterDoctorServiceCommunicator(registry);
        }

        private void RegisterDoctorServiceCommunicator(IContainerRegistry registry)
        {
            IConnectivity connectivity = Container.Resolve<IConnectivity>();
            IConfig config = Container.Resolve<IConfig>();
            ConnectionStatusManager connectionStatusManager = Container.Resolve<ConnectionStatusManager>();
            IGsmConnection gsmConnection = Container.Resolve<IGsmConnection>();
            ISmsTask smsTask = Container.Resolve<ISmsTask>();
            INativeHttpMessageHandlerProvider nativeHttpMessageHandlerProvider = Container.Resolve<INativeHttpMessageHandlerProvider>();

            var internetServiceCommunicator =
                new DoctorServiceCommunicatorViaInternet(connectivity, config, connectionStatusManager, nativeHttpMessageHandlerProvider);
            var smsServiceCommunicator = new DoctorServiceCommunicatorViaSms(connectivity, config, connectionStatusManager, nativeHttpMessageHandlerProvider, gsmConnection, smsTask);
            internetServiceCommunicator.SetNextCommunicator(smsServiceCommunicator); //Chain of responsibility pattern

            registry.RegisterInstance(typeof(IDoctorServiceCommunicator), internetServiceCommunicator);
        }

        private void SetLocale()
        {
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");

            CurrentLanguage = SupportedLanguages.Farsi;
        }
    }

    public static class ContainerRegistryExtension
    {
        public static void RegisterPerResolveLifeTime<TFrom, TTo>(this IContainerRegistry registry)
            where TFrom : class
            where TTo : class, TFrom
        {
            registry.GetContainer().RegisterType<TFrom, TTo>(new PerResolveLifetimeManager());
        }
    }
}