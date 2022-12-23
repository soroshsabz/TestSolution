using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage
	{
		public TestPage ()
		{
		    ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent ();
		}
	}
}