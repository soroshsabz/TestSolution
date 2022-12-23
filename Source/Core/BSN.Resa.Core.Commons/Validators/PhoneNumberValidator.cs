using PhoneNumbers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    //TODO: this regex will not suppor the international numbers
    //FIXME: support international numbers validation
    public abstract class PhoneNumberValidator
    {
        public const int Length = 11;
        private const string _defaultCityCode = "021";
        private const string _defaultInternationalStandardCityCode = "+9821";

        public static ValidationResult ValidatePhoneNumber(object value)
        {
            var number = value?.ToString() ?? "";

            //کد شهرستان عدد 11 رقمی می باشد که با 01 تا 08 شروع میشود			
            if (!new Regex(@"^(0098|98|0|\+98|)[1-9][0-9]{9}$|^[0-9]{8}$", RegexOptions.IgnoreCase).IsMatch(number))
                    return new ValidationResult(Locale.Resources.PhoneNumberInvalid);

            return ValidationResult.Success;
        }

        public static bool IsValidPhoneNumber(object value)
        {
            return ValidatePhoneNumber(value) == ValidationResult.Success;
        }

        public static bool Normalize(ref string phoneNumber)
        {
            try
            {
                phoneNumber = phoneNumber.Replace("+", "0");

                if (phoneNumber.StartsWith("98") && phoneNumber.Length > Length)
                {
                    phoneNumber = phoneNumber.Remove(0, 2).Insert(0, "0");
                }
                else if (phoneNumber.StartsWith("098") && phoneNumber.Length > Length)
                {
                    phoneNumber = phoneNumber.Remove(0, 3).Insert(0, "0");
                }
                else if (phoneNumber.Length == Length - 1)
                {
                    phoneNumber = phoneNumber.Insert(0, "0");
                }
                else if (phoneNumber.Length == Length - 3)
                {
                    phoneNumber = phoneNumber.Insert(0, _defaultCityCode);
                }

                if (!IsValidPhoneNumber(phoneNumber))
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Internationalize(string phoneNumber, string countryCode = "IR")
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            PhoneNumber number = phoneUtil.Parse(phoneNumber, countryCode);
            return phoneUtil.FormatOutOfCountryCallingNumber(number, string.Empty).Replace(" ", "");
        }

        public static bool NormalizeToInternationalStandardFormat(ref string phoneNumber)
        {
            try
            {
                if (!IsValidPhoneNumber(phoneNumber))
                    return false;

                if (phoneNumber.StartsWith("98") && phoneNumber.Length > Length)
                {
                    phoneNumber = phoneNumber.Insert(0, "+");
                }
                else if ((phoneNumber.StartsWith("0") || phoneNumber.StartsWith("+")) && phoneNumber.Length == Length)
                {
                    phoneNumber = phoneNumber.Remove(0, 1);
                    phoneNumber = phoneNumber.Insert(0, "+98");
                }
                else if (phoneNumber.StartsWith("098") && phoneNumber.Length > Length)
                {
                    phoneNumber = phoneNumber.Remove(0, 1).Insert(0, "+");
                }
                else if (phoneNumber.Length == Length - 1)
                {
                    phoneNumber = phoneNumber.Insert(0, "+98");
                }
                else if (phoneNumber.Length == Length - 3)
                {
                    phoneNumber = phoneNumber.Insert(0, _defaultInternationalStandardCityCode);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
