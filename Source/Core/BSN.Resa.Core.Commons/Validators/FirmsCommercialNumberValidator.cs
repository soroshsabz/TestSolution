using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class FirmsCommercialNumberValidator
    {
        public const int Length = 12;

        public static ValidationResult Validate(object value)
        {
            string number = value?.ToString() ?? "";

                // کد اقتصادی شماره ای است 12 رقمی که بر طبق قانون برای اشخاص حقیقی و حقوقی
                //که به تولید کالا و خدمات می پردازند وضع شده است 
                //این کد جهت شناسایی شرکت در واحد مالیاتی استفاده می شود.
                
                if (!new Regex(@"^[0-9]{" + Length + @"}$").IsMatch(number))
                return new ValidationResult(Locale.Resources.FirmsCommercialNumberInvalid);

            return ValidationResult.Success;
        }

        public static bool IsValid(object value)
        {
            return Validate(value) == ValidationResult.Success;
        }
    }
}
