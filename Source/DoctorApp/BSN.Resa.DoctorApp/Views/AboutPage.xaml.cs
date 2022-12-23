using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : BaseContentPage
    {
        public AboutPage()
        {
            ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent();
        }
    }
}