using BSN.Resa.Core.Commons.Validators;
using BSN.Resa.Locale;
using System;

namespace BSN.Resa.Core.Commons
{
    public static class StringExtension
	{
		public static string Right(this string str, int length)
		{
			if (str.Length >= length)
				return str.Substring(str.Length - length, length);
			return str;
		}

		public static string Left(this string str, int length)
		{
			if (str.Length >= length)
				return str.Substring(0, length);
			return str;
		}

        public static void ThrowExceptionIfIsNullOrWhiteSpace(this string str, string name)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException(Resources.CanNotBeBlank, name);
        }
    }
}
