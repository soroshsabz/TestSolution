using BSN.Resa.DoctorApp.ViewModels.Contacts;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ContactsListControl : ContentView
	{
		public ContactsListControl()
		{
			InitializeComponent();
		    FlowDirection = FlowDirection.LeftToRight;
		}

		public static readonly BindableProperty IsSearchModeProperty = BindableProperty.Create(
			nameof(IsSearchMode),
			typeof(bool),
			typeof(ContactsListControl),
			defaultValue: false);

		public static readonly BindableProperty SearchingPhoneNumberProperty = BindableProperty.Create(
			nameof(SearchingPhoneNumber),
			typeof(string),
			typeof(ContactsListControl),
			defaultValue: string.Empty);

		public static readonly BindableProperty ContactsProperty = BindableProperty.Create(
			nameof(Contacts),
			typeof(ObservableCollection<BaseContactsListPageViewModel.ContactItem>),
			typeof(ContactsListControl),
			defaultValue: null);

		public ObservableCollection<BaseContactsListPageViewModel.ContactItem> Contacts
		{
			get
			{
				var contacts =  (ObservableCollection<BaseContactsListPageViewModel.ContactItem>) GetValue(ContactsProperty);
				IsAnyPhoneNumber = contacts != null && contacts.Count != 0;
				OnPropertyChanged(nameof(IsAnyPhoneNumber));
				return contacts;
			}
		}

		public string SearchingPhoneNumber
		{
			get { return (string) GetValue(SearchingPhoneNumberProperty); }
			set
			{
				// TODO: Error handling, ArgumentException
				SetValue(SearchingPhoneNumberProperty, value);
			}
		}
		public bool IsSearchMode
		{
			get { return (bool) GetValue(IsSearchModeProperty); }
			set
			{
				// TODO: Error handling, ArgumentException
				SetValue(IsSearchModeProperty, value);
			}
		}

		public bool IsAnyPhoneNumber { get; private set; }
	}
}