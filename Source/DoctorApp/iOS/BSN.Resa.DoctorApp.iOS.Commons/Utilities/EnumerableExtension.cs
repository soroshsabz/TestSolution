using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.DoctorApp.iOS.Commons.Utilities
{
	public static class EnumerableExtension
	{
		public static string ToString<T>(this IEnumerable<T> enumerable, 
			string delimiter,
			bool appendDelimiterAtStart = false,
			bool appendDelimiterAtEnd = false)
		{
			var builder = new StringBuilder();

			int counter = 0;
			foreach (var element in enumerable)
			{
				if(counter == 0 && appendDelimiterAtStart)
					builder.Append(delimiter);
				else if (counter != 0)
					builder.Append(delimiter);
				builder.Append(element);
				++counter;
			}

			if (counter == 0 && appendDelimiterAtStart)
				builder.Append(delimiter);

			if (appendDelimiterAtEnd)
				builder.Append(delimiter);
;
			return builder.ToString();
		}
	}
}