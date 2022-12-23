using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.CallbackRequests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CallbackRequestsPage
    {
        public CallbackRequestsPage()
        {
            ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.Callback;

            InitializeComponent();
        }
    }
}