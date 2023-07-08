using System;

using UWPTest.ViewModels;

using Windows.UI.Xaml.Controls;

namespace UWPTest.Views
{
    public sealed partial class WebViewPage : Page
    {
        public WebViewViewModel ViewModel { get; } = new WebViewViewModel();

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);
        }
    }
}
