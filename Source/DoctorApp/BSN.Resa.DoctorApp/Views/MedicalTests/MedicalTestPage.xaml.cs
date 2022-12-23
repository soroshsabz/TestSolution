using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using Xamarin.Forms.Xaml;

namespace BSN.Resa.DoctorApp.Views.MedicalTests
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MedicalTestPage
    {
        public MedicalTestPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            CrossDeviceOrientation.Current.UnlockOrientation();
        }
    }
}