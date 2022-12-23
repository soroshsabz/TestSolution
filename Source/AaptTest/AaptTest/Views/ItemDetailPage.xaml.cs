using AaptTest.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace AaptTest.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}