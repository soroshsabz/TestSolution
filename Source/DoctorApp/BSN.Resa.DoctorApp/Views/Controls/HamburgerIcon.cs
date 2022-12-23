using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    public class HamburgerIcon : ResaToolbarItem
    {
        public HamburgerIcon()
        {
            HorizontalOptions = LayoutOptions.Start;
        }

        public override string Icon => "Assets/hamburger.png";

        protected override void Icon_OnClicked()
        {
            if (App.CustomFlyoutPage != null)
            {
                App.CustomFlyoutPage.IsPresented = !App.CustomFlyoutPage.IsPresented;
            }
        }
    }
}
