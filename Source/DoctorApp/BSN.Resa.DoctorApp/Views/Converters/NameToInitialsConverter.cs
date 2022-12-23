using System;
using System.Globalization;
using BSN.Resa.DoctorApp.Commons.Utilities;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.Views.Converters
{
    public class NameToInitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value as string;

            if (name.IsNullOrEmptyOrSpace())
                return "";

            return GetInitials(name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";

            var nameParts = name.Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries);
            var partsLength = nameParts.Length;

            try
            {
                switch (partsLength)
                {
                    case 0:
                    {
                        return "";
                    }
                    case 1:
                    {
                        return nameParts[0].Substring(0, 1);
                    }
                    case 2:
                    {
                        return nameParts[0].Substring(0, 1) + '\u2009' + nameParts[1].Substring(0, 1); //'\u2009' is half-space. For example ع‌ل
                        }

                    default:
                    {
                        return nameParts[0].Substring(0, 1) + '\u2009' + nameParts[2].Substring(0, 1);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}