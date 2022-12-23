using System;
using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppSettingsPage
    {
        public AppSettingsPage()
        {
            ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent();
        }

        private void AdvancedSettingsRow_OnTapped(object sender, EventArgs e)
        {
//            if(AdvancedSettingsRow == null) return;
//            AdvancedSettingsRow.Opacity = 0.3;
//            AdvancedSettingsRow.FadeTo(1, 2000);
//            AdvancedSettingsRow.ScaleTo(0.1, 300).ConfigureAwait(false);
//            AdvancedSettingsRow.ScaleTo(1, 50).ConfigureAwait(false);
        }
    }
}