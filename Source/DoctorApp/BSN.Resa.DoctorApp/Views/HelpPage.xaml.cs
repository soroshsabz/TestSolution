using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HelpPage : BaseContentPage
	{
		public HelpPage()
		{
		    ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent();
		}
	}
}