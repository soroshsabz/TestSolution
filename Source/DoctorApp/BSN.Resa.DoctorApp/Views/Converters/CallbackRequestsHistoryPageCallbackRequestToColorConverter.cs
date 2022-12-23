using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class CallbackRequestsHistoryPageCallbackRequestToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callbackRequestWrapper = value as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;

            if (callbackRequest == null) return Color.Black;

            if (callbackRequest.ReturnCallHasBeenEstablished)
                return GreenColor;

            return RedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static readonly Color GreenColor = Color.FromHex("#2ecc71");
        private static readonly Color RedColor = Color.FromHex("#B71C1C");
    }
}
