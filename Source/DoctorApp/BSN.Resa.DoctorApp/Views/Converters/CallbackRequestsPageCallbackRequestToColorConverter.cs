using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class CallbackRequestsPageCallbackRequestToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callbackRequestWrapper = value as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;

            if (callbackRequest == null) return Color.Black;

            if (callbackRequest.ReturnCallHasBeenEstablished)
                return GreenColor;

            if (callbackRequest.IsCallTried)
                return Color.Gray;

            var group = CallbackRequestsBaseViewModel.CalculateCallbackRequestGroup(callbackRequestWrapper);

            switch (group)
            {
                case CallbackRequestsBaseViewModel.CallbackRequestGroupList.CallbackRequestGroup.Today:
                {
                    return Color.FromHex("#B71C1C");
                }
                case CallbackRequestsBaseViewModel.CallbackRequestGroupList.CallbackRequestGroup.Yesterday:
                {
                    return Color.FromHex("#E53935");
                }
                default:
                {
                    return Color.FromHex("#E57373");
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static readonly Color GreenColor = (Color)Application.Current.Resources["AppPrimaryGreenColor"];
    }
}