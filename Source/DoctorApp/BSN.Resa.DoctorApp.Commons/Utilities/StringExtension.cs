using BSN.Resa.Locale;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.DoctorApp.Commons.Utilities
{
    public static class StringExtension
    {
        public static string[] ToE164PhoneNumberFormat(this string[] phoneNumbers, string defaultRegion = "IR")
        {
            string[] output = new string[phoneNumbers.Length];
            for (int i = 0; i < output.Length; ++i)
                output[i] = phoneNumbers[i].ToE164PhoneNumberFormat(defaultRegion);
            return output;
        }

        public static string[] ToNationalPhoneNumberFormat(this string[] phoneNumbers, string defaultRegion = "IR")
        {
            string[] output = new string[phoneNumbers.Length];
            for (int i = 0; i < output.Length; ++i)
                output[i] = phoneNumbers[i].ToNationalPhoneNumberFormat(defaultRegion);
            return output;
        }

        public static string ToE164PhoneNumberFormat(this string phoneNumber, string defaultRegion = "IR")
        {
            string parsedPhoneNumber = PhoneNumberUtil.GetInstance()
                                            .Format(PhoneNumberUtil.GetInstance().Parse(phoneNumber, defaultRegion), PhoneNumberFormat.E164);

            if (!IsValidPhoneNumber(parsedPhoneNumber))
                throw new NumberParseException(ErrorType.NOT_A_NUMBER, "The phone number supplied is not valid.");

            return parsedPhoneNumber;
        }

        public static string ToNationalPhoneNumberFormat(this string phoneNumber, string defaultRegion = "IR")
        {
            string parsedPhoneNumber = PhoneNumberUtil.GetInstance()
                .Format(PhoneNumberUtil.GetInstance().Parse(phoneNumber, defaultRegion), PhoneNumberFormat.NATIONAL).Replace(" ", string.Empty);

            if (!IsValidPhoneNumber(parsedPhoneNumber))
                throw new NumberParseException(ErrorType.NOT_A_NUMBER, "The phone number supplied is not valid.");

            return parsedPhoneNumber;
        }

        /// <summary>
        /// What is difference between IsPossiblePhoneNumber(number) and IsValidPhoneNumber(number) ?
        /// According to official doc IsPossiblePhoneNumber(number) provides a more lenient check than
        /// IsValidPhoneNumber(number). For more details
        /// <a href="https://github.com/google/libphonenumber/blob/master/java/libphonenumber/src/com/google/i18n/phonenumbers/PhoneNumberUtil.java#L2604">visit official doc.</a>
        /// <br></br>
        /// Refer specifically to the documentation of the method: <i>public ValidationResult isPossibleNumberWithReason(PhoneNumber number)</i>
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="defaultRegion"></param>
        /// <returns></returns>
        public static bool IsValidPhoneNumber(this string phoneNumber, string defaultRegion = "IR")
        {
            try
            {
                bool isValidForSelectedRegion = PhoneNumberUtil.GetInstance().IsValidNumber(PhoneNumberUtil.GetInstance().Parse(phoneNumber, defaultRegion));
                bool isValidForOmanRegion = PhoneNumberUtil.GetInstance().IsValidNumber(PhoneNumberUtil.GetInstance().Parse(phoneNumber, "OM"));

                return isValidForSelectedRegion || isValidForOmanRegion;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }

        public static IEnumerable<string> FindPhoneNumbers(this string text, string defaultRegion = "IR")
        {
            return PhoneNumberUtil
                    .GetInstance()
                    .FindNumbers(text, defaultRegion)
                    .Select(phoneNumberMatch => phoneNumberMatch.RawString);
        }

        public static T? To<T>(this string enumValue) where T : struct, IConvertible // enum
        {
            var values = Enum.GetValues(typeof(T));
            foreach (T value in values)
            {
                if (enumValue == value.ToString() || enumValue == Resources.ResourceManager.GetString(value.ToString()))
                    return value;
            }
            return null;
        }

        public static bool IsNullOrEmptyOrSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNumber(this string input)
        {
            if (input.IsNullOrEmptyOrSpace())
                throw new ArgumentException();

            return input.All(char.IsDigit);
        }

        private static char ToPersianDigit(char englishDigit)
        {
            if (!char.IsDigit(englishDigit))
                throw new ArgumentException();

            char result;
            switch (englishDigit)
            {
                case '0':
                    {
                        result = '۰';
                        break;
                    }
                case '1':
                    {
                        result = '۱';
                        break;
                    }
                case '2':
                    {
                        result = '۲';
                        break;
                    }
                case '3':
                    {
                        result = '۳';
                        break;
                    }
                case '4':
                    {
                        result = '۴';
                        break;
                    }
                case '5':
                    {
                        result = '۵';
                        break;
                    }
                case '6':
                    {
                        result = '۶';
                        break;
                    }
                case '7':
                    {
                        result = '۷';
                        break;
                    }
                case '8':
                    {
                        result = '۸';
                        break;
                    }
                case '9':
                    {
                        result = '۹';
                        break;
                    }
                default:
                    {
                        result = ' ';
                        break;
                    }
            }

            return result;
        }

        public static string ToPersianNumber(this string englishNumber)
        {
            if (!englishNumber.IsNumber())
                throw new ArgumentException();

            var persianNumber = englishNumber.Select(ToPersianDigit);
            return new string(persianNumber.ToArray());
        }
    }
}

