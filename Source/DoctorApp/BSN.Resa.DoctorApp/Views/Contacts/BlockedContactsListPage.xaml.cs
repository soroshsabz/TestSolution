using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.Contacts
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BlockedContactsListPage : BaseContentPage
	{
		public BlockedContactsListPage()
		{
		    ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent();
		}
	}
}
