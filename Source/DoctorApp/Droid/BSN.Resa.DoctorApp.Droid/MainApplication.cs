using Android.App;
using Android.OS;
using Android.Runtime;
using BSN.Resa.DoctorApp.Utilities;
using Plugin.CurrentActivity;
using System;
using System.Net;

namespace BSN.Resa.DoctorApp.Droid
{
    //You can specify additional application information in this attribute
    //More info about Debuggable parameter: https://stackoverflow.com/a/37148535/5941852
    //Note: The parameters set here will be inserted in AndroidManifest.xml automatically.
    [Application(
#if DEBUG
        Debuggable = true
#else
    Debuggable = false
#endif
    )]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        #region Constructor

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        #endregion

        #region Life-cycle Methods

        public override void OnCreate()
        {
            DisablerAndroidMessageHandlerEmitter.Register();
            DisablerTrustProvider.Register();

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            base.OnCreate();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            InitializeAppCenter();

            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            DisablerAndroidMessageHandlerEmitter.Register();
            DisablerTrustProvider.Register();

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
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
            AppCenterInitializer.Init(ConfigAndroid.Instance);
        }

        #endregion
    }
}