using System;
using System.Globalization;
using BSN.Resa.Locale;
using MD.PersianDateTime.Standard;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class StringDateTimeToFancyPersianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            bool isDateTime = value is DateTime;

            if (!isDateTime)
                return string.Empty;

            var dateTime = (DateTime)value;

            var result = CalculateDelta(dateTime);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string CalculateDelta(DateTime dateTime)
        {
            var now = DateTime.Now;

            var delta = now - dateTime;

            if (delta.Days == 0)
            {
                return CalculateDeltaForToday(dateTime);
            }

            if (delta.Days == 1)
            {
                return CalculateDeltaForYesterday(dateTime);
            }

            return CalculateDeltaForOlders(dateTime);
        }

        private string CalculateDeltaForYesterday(DateTime dateTime)
        {
            string result;
            //TODO: change here in case of adding more supported languages
            if (App.CurrentLanguage == SupportedLanguages.Farsi)
                result = "دیروز " + new PersianDateTime(dateTime).ToShortTimeString();
            else if (App.CurrentLanguage == SupportedLanguages.English)
                result = "Yesterday at " + dateTime.ToShortTimeString();
            else
                result = "دیروز " + new PersianDateTime(dateTime).ToShortTimeString();

            return result;
        }

        private string CalculateDeltaForToday(DateTime dateTime)
        {
            var now = DateTime.Now;

            var delta = now - dateTime;

            string result;

            if (delta.Hours == 0 && delta.Minutes == 0)
            {
                result = Resources.Now;
            }
            else if (delta.Hours == 0)
            {
                result = delta.Minutes + " " + Resources.MinutesAgo;
            }
            else if (delta.Minutes == 0)
            {
                result = delta.Hours + " " + Resources.HoursAgo;
            }
            else
            {
                //TODO: change here in case of adding more supported languages
                if (App.CurrentLanguage == SupportedLanguages.English)
                    result = $"{delta.Hours} and {delta.Minutes} minutes ago";
                else
                    result = $"{delta.Hours} ساعت و {delta.Minutes} دقیقه قبل";
            }

            return result;
        }

        private string CalculateDeltaForOlders(DateTime dateTime)
        {
            if (App.CurrentLanguage == SupportedLanguages.English)
                return ToFancyGregorianDateTime(dateTime);
            else
                return new PersianDateTime(dateTime).ToShortDateTimeString();
        }

        private string ToFancyGregorianDateTime(DateTime dateTime)
        {
            string result = $"{dateTime.ToLongDateString()} at {dateTime.ToShortTimeString()}";
            return result;
        }
    }
}