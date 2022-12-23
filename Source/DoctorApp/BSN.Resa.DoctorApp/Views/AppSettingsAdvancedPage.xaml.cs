using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppSettingsAdvancedPage
	{
		public AppSettingsAdvancedPage ()
		{
		    ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent ();
		}
	}
}