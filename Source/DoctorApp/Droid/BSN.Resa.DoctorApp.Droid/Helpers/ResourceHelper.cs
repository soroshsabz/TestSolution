using System;
using System.IO;
using Android.Content;

namespace BSN.Resa.DoctorApp.Droid.Helpers
{
	public static class ResourceHelper
	{
		private static Context Context => Android.App.Application.Context;

		// Assets/image.png => image
		// Drawable/image.png => image
		// @Drawable/image.png => image
		public static string ResourceNameWithoutExtension(string resourcePath)
		{
			const string assets = "assets/";
			const string drawable = "drawable/";
			const string atDrawable = "@drawable/";
			string filename;
			string lowerResourcePath = resourcePath.ToLower();
			if (lowerResourcePath.StartsWith(assets))
				filename = resourcePath.Remove(0, assets.Length);
			else if (lowerResourcePath.StartsWith(drawable))
				filename = resourcePath.Remove(0, drawable.Length);
			else if(lowerResourcePath.Contains(atDrawable))
				filename = resourcePath.Remove(0, atDrawable.Length);
			else
				filename = resourcePath;
			return RemoveExtension(filename);
		}

		public static byte[] ReadAllBytes(string resourcePath, string resourceType)
		{
			byte[] resourceData;
			using (var memoryStream = new MemoryStream())
			{
				int resourceId = Context.Resources.GetIdentifier(ResourceNameWithoutExtension(resourcePath), resourceType,
					Context.PackageName);
				Stream resourceStream = Context.Resources.OpenRawResource(resourceId);
				resourceStream.CopyTo(memoryStream);
				resourceData = memoryStream.ToArray();
			}
			return resourceData;
		}

		private static string RemoveExtension(string path)
		{
			return path.Remove(path.LastIndexOf(".", StringComparison.Ordinal));
		}
	}
}