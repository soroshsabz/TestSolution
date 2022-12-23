using BSN.Resa.DoctorApp.Views.Controls;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.Contacts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AllowedContactsListPage : BaseContentPage
	{
		public AllowedContactsListPage()
		{
		    ResaBottomNavigationBar.CurrentTab = ResaBottomToolbarTab.UnSelected;

            InitializeComponent();
        }
	}
}
