using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.CallbackRequests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CallbackRequestsPageListView
    {
        #region BindableProperties

        public static readonly BindableProperty CallCommandProperty = BindableProperty.Create(nameof(CallCommand),
            typeof(ICommand), typeof(CallbackRequestsPageListView));

        public static readonly BindableProperty OnItemAppearingCommandProperty = BindableProperty.Create(nameof(OnItemAppearingCommand),
            typeof(ICommand), typeof(CallbackRequestsPageListView));

        public static readonly BindableProperty IsCallVisibleProperty = BindableProperty.Create(nameof(IsCallVisible),
            typeof(bool), typeof(CallbackRequestsPageListView));
        #endregion

        public CallbackRequestsPageListView()
        {
            InitializeComponent();

            SetBinding(CallCommandProperty, new Binding("CallCommand"));
        }

        public ICommand CallCommand
        {
            get => (ICommand)GetValue(CallCommandProperty);
            set => SetValue(CallCommandProperty, value);
        }

        public bool IsCallVisible
        {
            get => (bool)GetValue(IsCallVisibleProperty);
            set => SetValue(IsCallVisibleProperty, value);
        }

        public ICommand OnItemAppearingCommand
        {
            get => (ICommand)GetValue(OnItemAppearingCommandProperty);
            set => SetValue(OnItemAppearingCommandProperty, value);
        }
    }
}