using Microsoft.UI.Xaml.Controls;

using WinUI3.ViewModels;

namespace WinUI3.Views;

// To learn more about WebView2, see https://docs.microsoft.com/microsoft-edge/webview2/.
public sealed partial class SampleWebViewPage : Page
{
    public SampleWebViewViewModel ViewModel
    {
        get;
    }

    public SampleWebViewPage()
    {
        ViewModel = App.GetService<SampleWebViewViewModel>();
        InitializeComponent();

        ViewModel.WebViewService.Initialize(WebView);
    }
}
