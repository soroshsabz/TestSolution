using BSN.Resa.Locale;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class CallbackRequestCreditToFancyExpressionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long? credit = value as long?;

            if(credit == null)
                return "";
            
            if(credit > 1)
                return credit + " " + Resources.Minutes;

            return credit + " " + Resources.Minute;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}