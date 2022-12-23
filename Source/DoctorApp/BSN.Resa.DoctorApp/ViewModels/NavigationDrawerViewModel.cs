using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Views;
using BSN.Resa.DoctorApp.Views.CallbackRequests;
using BSN.Resa.DoctorApp.Views.Contacts;
using BSN.Resa.DoctorApp.Views.MedicalTests;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace BSN.Resa.DoctorApp.ViewModels
{
    public class NavigationDrawerViewModel : BindableBase
    {
        #region Constructor

        public NavigationDrawerViewModel(
            IConfig config,
            INavigationService navigationService
        )
        {
            _navigationService = navigationService;
            _config = config;

            Menus = new ObservableCollection<MenuItem>();

            InitMenus();
        }

        #endregion

        public bool IsPresented
        {
            get => _isPresented;
            set => SetProperty(ref _isPresented, value);
        }

        public MenuItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);

                PageChange(value);
            }
        }

        public ObservableCollection<MenuItem> Menus { get; }

        #region Private Methods

        private void InitMenus()
        {
            Menus.Add(new MenuItem
            {
                Title = Locale.Resources.MainPage,
                PageName = nameof(CallbackRequestsPage),
                IconName = "Assets/home.png"
            });

            if (_config.HasCallBlockingFeature)
            {
                Menus.Add(new MenuItem
                {
                    Title = Locale.Resources.BlockedNumbers,
                    PageName = nameof(BlockedContactsListPage),
                    IconName = "Assets/blocked.png"
                });

                Menus.Add(new MenuItem
                {
                    Title = Locale.Resources.AllowedNumbers,
                    PageName = nameof(AllowedContactsListPage),
                    IconName = "Assets/allowed.png"
                });
            }

            Menus.Add(new MenuItem
            {
                Title = Locale.Resources.DoctorAppMedicalTests,
                PageName = nameof(ActiveMedicalTestsPage),
                IconName = "Assets/medical_tests.png"
            });

            if (_config.TargetPlatform == MobilePlatform.iOS && _config.HasCallBlockingFeature)
            {
                Menus.Add(new MenuItem
                {
                    Title = Locale.Resources.Help,
                    PageName = nameof(HelpPage),
                    IconName = "Assets/help.png"
                });
            }

            if (_config.TargetPlatform == MobilePlatform.Android)
            {
                Menus.Add(new MenuItem
                {
                    Title = Locale.Resources.Settings,
                    PageName = nameof(AppSettingsPage),
                    IconName = "Assets/settings.png"
                });
            }

            Menus.Add(new MenuItem
            {
                Title = Locale.Resources.About,
                PageName = nameof(AboutPage),
                IconName = "Assets/about.png"
            });

#if DEBUG
            Menus.Add(new MenuItem
            {
                Title = "تست",
                PageName = nameof(TestPage),
                IconName = "Assets/icon.png"
            });
#endif
        }

        private async void PageChange(MenuItem menuItem)
        {
            await _navigationService.NavigateAsync(
                $"/{nameof(FlyoutPage)}/{nameof(AppNavigationPage)}/{menuItem.PageName}");

            IsPresented = false;
        }

        #endregion

        #region Private Fields

        private MenuItem _selectedItem;
        private readonly INavigationService _navigationService;
        private readonly IConfig _config;
        private bool _isPresented;

        #endregion
    }

    public class MenuItem
    {
        public string Title { get; set; }

        public string PageName { get; set; }

        public string IconName { get; set; }
    }
}