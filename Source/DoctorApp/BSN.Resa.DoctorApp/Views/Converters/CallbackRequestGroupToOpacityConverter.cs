using BSN.Resa.DoctorApp.ViewModels.CallbackRequests;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class CallbackRequestGroupToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var callbackRequestWrapper = value as CallbackRequestBindableObject;
            var callbackRequest = callbackRequestWrapper?.CallbackRequest;

            if (callbackRequest == null)
                return 1;

            var group = CallbackRequestsBaseViewModel.CalculateCallbackRequestGroup(callbackRequestWrapper);

            if (group == CallbackRequestsBaseViewModel.CallbackRequestGroupList.CallbackRequestGroup.Yesterday)
                return 0.6;

            return 1;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}