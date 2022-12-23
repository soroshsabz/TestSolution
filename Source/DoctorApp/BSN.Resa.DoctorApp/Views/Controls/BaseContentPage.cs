using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    /// <summary>
    /// This class is intended to be inherited by all pages so that making changes to all pages at once would be easier
    /// </summary>
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            FlowDirection = FlowDirection.RightToLeft;

            On<iOS>().SetUseSafeArea(true);
        }
    }
}
