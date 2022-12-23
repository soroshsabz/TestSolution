using System;
using System.Globalization;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class CallbackRequestMessageToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callbackRequestWrapper = value as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;

            if (callbackRequest == null || callbackRequest.Message.IsNullOrEmptyOrSpace()) return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
