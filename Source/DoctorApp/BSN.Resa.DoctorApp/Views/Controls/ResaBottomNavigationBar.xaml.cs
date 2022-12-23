using System;
using System.Windows.Input;
using BSN.Resa.DoctorApp.Views.CallbackRequests;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResaBottomNavigationBar
    {
        public static ResaBottomToolbarTab CurrentTab = ResaBottomToolbarTab.UnSelected;

        #region Bindable Properties

        public static readonly BindableProperty BadgeNumberProperty = BindableProperty.Create(nameof(BadgeNumber),
            typeof(int), typeof(ResaBottomNavigationBar));

        public static readonly BindableProperty IsBadgeNumberVisibleProperty = BindableProperty.Create(nameof(IsBadgeNumberVisible),
            typeof(bool), typeof(ResaBottomNavigationBar), defaultValue: false);

        public static readonly BindableProperty DoctorStateTabSelectedCommandProperty = BindableProperty.Create(nameof(DoctorStateTabSelectedCommand),
            typeof(ICommand), typeof(ResaBottomNavigationBar));

        public static readonly BindableProperty CallbackTabSelectedCommandProperty = BindableProperty.Create(nameof(CallbackTabSelectedCommand),
            typeof(ICommand), typeof(ResaBottomNavigationBar));

        public static readonly BindableProperty CallHistoryTabSelectedCommandProperty = BindableProperty.Create(nameof(CallHistoryTabSelectedCommand),
            typeof(ICommand), typeof(ResaBottomNavigationBar));

        #endregion

        public ResaBottomNavigationBar()
        {
            InitializeComponent();

            _appPrimaryColor = (Color) Application.Current.Resources["AppPrimaryColor"];

            //Instead of setting binding on BadgeNumber/IsBadgeNumberVisible properties on every
            //page that has bottom bar, we do that here once!
            //It means The correspondent viewmodel properties exist in BaseViewModel class which is inherited by all those pages.
            SetBinding(BadgeNumberProperty, new Binding("UnSeenCallbackCount"));
            SetBinding(IsBadgeNumberVisibleProperty, new Binding("IsCallbackCountVisible"));

            SetBinding(DoctorStateTabSelectedCommandProperty, new Binding("ResaBottomBarTabSelectedCommand"));
            SetBinding(CallbackTabSelectedCommandProperty, new Binding("ResaBottomBarTabSelectedCommand"));
            SetBinding(CallHistoryTabSelectedCommandProperty, new Binding("ResaBottomBarTabSelectedCommand"));

            SelectTab(CurrentTab, executeCommand: false); //Select tab only UI-wise
        }

        public int BadgeNumber
        {
            get => (int)GetValue(BadgeNumberProperty);
            set => SetValue(BadgeNumberProperty, value);
        }

        public bool IsBadgeNumberVisible
        {
            get => (bool)GetValue(IsBadgeNumberVisibleProperty);
            set => SetValue(IsBadgeNumberVisibleProperty, value);
        }

        public ICommand DoctorStateTabSelectedCommand
        {
            get => (ICommand)GetValue(DoctorStateTabSelectedCommandProperty);
            set => SetValue(DoctorStateTabSelectedCommandProperty, value);
        }

        public ICommand CallbackTabSelectedCommand
        {
            get => (ICommand)GetValue(CallbackTabSelectedCommandProperty);
            set => SetValue(CallbackTabSelectedCommandProperty, value);
        }

        public ICommand CallHistoryTabSelectedCommand
        {
            get => (ICommand)GetValue(CallHistoryTabSelectedCommandProperty);
            set => SetValue(CallHistoryTabSelectedCommandProperty, value);
        }

        private void SelectTab(ResaBottomToolbarTab tab, bool executeCommand = true)
        {
            switch (tab)
            {
                case ResaBottomToolbarTab.DoctorState:
                    {
                        ChangeStateLabel.TextColor = _appPrimaryColor;
                        CallHistoryLabel.TextColor = Color.Black;
                        CallbackLabel.TextColor = Color.Black;

                        DoctorStateImage.Source = "Assets/change_state_selected.png";
                        CallHistoryImage.Source = "Assets/call_history_unselected.png";
                        CallbackImage.Source = "Assets/callback_unselected.png";

                        CallHistoryAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        CallbackAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);

                        StateAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        StateAbsLayout.ScaleTo(1, 100).ConfigureAwait(false);

                        break;
                    }

                case ResaBottomToolbarTab.Callback:
                    {
                        CallbackLabel.TextColor = _appPrimaryColor;
                        CallHistoryLabel.TextColor = Color.Black;
                        ChangeStateLabel.TextColor = Color.Black;

                        CallbackImage.Source = "Assets/callback_selected.png";
                        DoctorStateImage.Source = "Assets/change_state_unselected.png";
                        CallHistoryImage.Source = "Assets/call_history_unselected.png";

                        CallHistoryAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        StateAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);

                        CallbackAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        CallbackAbsLayout.ScaleTo(1, 100).ConfigureAwait(false);
                        break;
                    }
                case ResaBottomToolbarTab.CallHistory:
                    {
                        CallHistoryLabel.TextColor = _appPrimaryColor;
                        ChangeStateLabel.TextColor = Color.Black;
                        CallbackLabel.TextColor = Color.Black;

                        CallHistoryImage.Source = "Assets/call_history_selected.png";
                        DoctorStateImage.Source = "Assets/change_state_unselected.png";
                        CallbackImage.Source = "Assets/callback_unselected.png";

                        StateAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        CallbackAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);

                        CallHistoryAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        CallHistoryAbsLayout.ScaleTo(1, 100).ConfigureAwait(false);
                        break;
                    }
                default:
                    {
                        CallHistoryLabel.TextColor = Color.Black;
                        ChangeStateLabel.TextColor = Color.Black;
                        CallbackLabel.TextColor = Color.Black;

                        CallHistoryImage.Source = "Assets/call_history_unselected.png";
                        DoctorStateImage.Source = "Assets/change_state_unselected.png";
                        CallbackImage.Source = "Assets/callback_unselected.png";

                        StateAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        CallbackAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        CallHistoryAbsLayout.ScaleTo(0.8, 50).ConfigureAwait(false);
                        break;
                    }
            }

            if (executeCommand)
                ExecuteTabSelectedCommand(tab);
        }

        private void ExecuteTabSelectedCommand(ResaBottomToolbarTab tab)
        {
            switch (tab)
            {
                case ResaBottomToolbarTab.DoctorState:
                    {
                        if (DoctorStateTabSelectedCommand == null || !DoctorStateTabSelectedCommand.CanExecute(null))
                            return;
                        DoctorStateTabSelectedCommand.Execute(nameof(DoctorStatePage));
                        break;
                    }
                case ResaBottomToolbarTab.Callback:
                    {
                        if (CallbackTabSelectedCommand == null || !CallbackTabSelectedCommand.CanExecute(null))
                            return;
                        CallbackTabSelectedCommand.Execute(nameof(CallbackRequestsPage));
                        break;
                    }
                case ResaBottomToolbarTab.CallHistory:
                    {
                        if (CallHistoryTabSelectedCommand == null || !CallHistoryTabSelectedCommand.CanExecute(null))
                            return;
                        CallHistoryTabSelectedCommand.Execute(nameof(CallbackRequestsHistoryPage));
                        break;
                    }
            }
        }

        private void StateAbsLayout_OnTapped(object sender, EventArgs e)
        {
            CurrentTab = ResaBottomToolbarTab.DoctorState;
            SelectTab(ResaBottomToolbarTab.DoctorState);
        }

        private void CallHistoryAbsLayout_OnTapped(object sender, EventArgs e)
        {
            CurrentTab = ResaBottomToolbarTab.CallHistory;
            SelectTab(ResaBottomToolbarTab.CallHistory);
        }

        private void CallbackAbsLayout_OnTapped(object sender, EventArgs e)
        {
            CurrentTab = ResaBottomToolbarTab.Callback;
            SelectTab(ResaBottomToolbarTab.Callback);
        }

        private readonly Color _appPrimaryColor;
    }

    public enum ResaBottomToolbarTab
    {
        UnSelected,
        DoctorState,
        Callback,
        CallHistory
    }
}