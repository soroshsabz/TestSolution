using System;

namespace BSN.Resa.Commons.General
{
	public static class UriExtension
	{
		public static string BaseAddress(this Uri uri)
		{
			return uri.GetLeftPart(UriPartial.Authority);
		}

		public static string QueryWithoutQuestionMark(this Uri uri)
		{
			return uri.Query.StartsWith("?") ? uri.Query.Substring(1) : uri.Query;
		}
	}
}
