using Android.Content;
using Android.Views;
using BSN.Resa.DoctorApp.Droid.CustomRenderers;
using BSN.Resa.DoctorApp.Utilities;
using BSN.Resa.DoctorApp.Views.Controls;
using System;
using BSN.Resa.DoctorApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomFlyoutPage), typeof(CustomFlyoutPageRenderer))]
namespace BSN.Resa.DoctorApp.Droid.CustomRenderers
{
    /// <summary>
    /// This custom renderer is used to make FlyoutPage(NavigationDrawer) to open from right to left.
    /// See: https://stackoverflow.com/questions/49026681/sliding-drawer-open-from-right-to-left-in-xamarin-android
    /// </summary>
    public class CustomFlyoutPageRenderer : FlyoutPageRenderer
    {
        public CustomFlyoutPageRenderer(Context context) : base(context)
        {
            _crashReporter = DependencyInjectionHelper.Resolve<ICrashReporter>();
        }

        protected override void OnElementChanged(VisualElement oldElement, VisualElement newElement)
        {
            base.OnElementChanged(oldElement, newElement);
            try
            {
                var fieldInfo = GetType().BaseType?.GetField("_masterLayout", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var masterLayout = (ViewGroup)fieldInfo?.GetValue(this);
                
                var lp = new LayoutParams(masterLayout?.LayoutParameters)
                {
                    Gravity = (int)GravityFlags.Right,
                };

                if (Device.Idiom == TargetIdiom.Tablet)
                {
                    lp.Width = 420;
                }
                if (masterLayout != null)
                {
                    masterLayout.LayoutParameters = lp;
                }
            }
            catch (Exception exception)
            {
                _crashReporter.SendException(exception);
            }
        }

        private readonly ICrashReporter _crashReporter;
    }
}