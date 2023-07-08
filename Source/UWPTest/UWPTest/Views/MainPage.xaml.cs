using System;

using UWPTest.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UWPTest.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
