using System;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Core.Commons.Validators
{
    public abstract class ChargeAmountValidator
    {
        private const int TomanRialEqualityRate = 10;

        public static ValidationResult ValidateAmount(object value)
        {
            if (!(value is int))
                return new ValidationResult(Locale.Resources.InvalidAmount);

            int amount;
            amount = (int)value;

            if (HasRialTrailingNotZero(amount))
                return new ValidationResult(Locale.Resources.InvalidAmountTailingZero);

            if (IsInvalidAmount(amount))
                return new ValidationResult(Locale.Resources.InvalidAmount);

            return ValidationResult.Success;
        }

        private static bool HasRialTrailingNotZero(int amount)
        {
            return amount % TomanRialEqualityRate != 0;
        }

        private static bool IsInvalidAmount(int amount)
        {
            if (amount <= 0)
                return true;

            if (!Enum.IsDefined(typeof(ChargeDenomination), amount))
                return true;

            return false;
        }
    }
}
