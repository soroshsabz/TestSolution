using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;
using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Utilities
{
	public static class EnumTypeExtension
	{
		public static List<string> ValuesAsLocalizedStrings(this Type enumType) 
		{
			Array doctorStateValues = Enum.GetValues(enumType);
			
			var doctorStateValuesWithLocale = new List<string>(doctorStateValues.Length);
			foreach (DoctorState state in doctorStateValues)
				doctorStateValuesWithLocale.Add(state.ToLocalizedString());

			return doctorStateValuesWithLocale;
		}

		public static string ToLocalizedString<T>(this T t) where T: struct, IConvertible
		{
			return Locale.Resources.ResourceManager.GetString(t.ToString());
		}
	}
}
