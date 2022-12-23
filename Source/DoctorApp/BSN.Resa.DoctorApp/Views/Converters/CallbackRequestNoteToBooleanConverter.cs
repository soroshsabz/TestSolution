using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using System;
using System.Globalization;
using BSN.Resa.DoctorApp.Commons.Utilities;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class CallbackRequestNoteToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callbackRequestWrapper = value as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;

            if (callbackRequest == null) return false;

            return !callbackRequest.Message.IsNullOrEmptyOrSpace();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}