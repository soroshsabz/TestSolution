using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.EventConsumers.InternetAccessedConsumers;
using BSN.Resa.DoctorApp.iOS.Commons;
using BSN.Resa.DoctorApp.iOS.EventConsumers.AppStartConsumers;
using BSN.Resa.DoctorApp.iOS.EventConsumers.ContactsChangedConsumers;
using BSN.Resa.DoctorApp.iOS.EventConsumers.UrlCallConsumers;
using BSN.Resa.DoctorApp.Utilities;
using FFImageLoading.Forms.Platform;
using Foundation;
using PanCardView.iOS;
using Plugin.Connectivity.Abstractions;
using SQLitePCL;
using System;
using UIKit;
using Xamarin.Forms;
using static BSN.Resa.DoctorApp.Utilities.DependencyInjectionHelper;

namespace BSN.Resa.DoctorApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        #region Public Methods

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();

            Batteries_V2.Init();

            CardsViewRenderer.Preserve();

            CachedImageRenderer.Init();

            LoadApplication(new App(new DependenciesRegistrationInitializeriOS()));

            InitializeAppCenter();

            RegisterDependencies();

            TriggerAppStart();

            RegisterInternetAccessedEventConsumers();

            RegisterContactsChangedConsumers();

            return base.FinishedLaunching(app, options);
        }

        /// <summary>
        /// Visit: https://github.com/wcoder/Xamarin.Plugin.DeviceOrientation/blob/master/README.md
        /// </summary>
        [Export("application:supportedInterfaceOrientationsForWindow:")]
        public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, IntPtr forWindow)
        {
            return Plugin.DeviceOrientation.DeviceOrientationImplementation.SupportedInterfaceOrientations;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return Resolve<IUrlCallConsumer>().OnUrlCall(url);
        }

        #endregion

        #region Private Methods

        private void TriggerAppStart()
        {
            Resolve<IAppStartConsumer>().OnStart();
        }

        private void RegisterContactsChangedConsumers()
        {
            Doctor.OnContactsChanged += (contacts) =>
            {
                Resolve<IContactsChangedConsumer>().OnContactsChanged(contacts);
            };
        }

        private void RegisterInternetAccessedEventConsumers()
        {
            Resolve<IConnectivity>().ConnectivityChanged += (sender, e) =>
            {
                if (!e.IsConnected)
                    return;

                Resolve<IInternetAccessedConsumer>().OnInternetAccessed();
            };
        }

        private void InitializeAppCenter()
        {
            //Why we should start AppCenter in MainApplication's OnCreate() method, and not in
            //App.cs class OnStart() method (which is the normal way)?
            //Because our application has multiple entry points other than App.cs, such as
            //background service, broadcast receiver, etc, and here is the centralized place for all.
            //Visit: https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin
            AppCenterInitializer.Init(ConfigiOS.Instance);
        }

        #endregion
    }
}

