using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.MedicalTests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveMedicalTestsPage
    {
        public ActiveMedicalTestsPage()
        {
            ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent();
        }
    }
}