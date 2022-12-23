using BSN.Resa.DoctorApp.iOS.Commons;
using BSN.Resa.DoctorApp.Utilities;
using FFImageLoading.Forms.Platform;
using Foundation;
using PanCardView.iOS;
using SQLitePCL;
using System;
using UIKit;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets
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

        #endregion

        #region Private Methods

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