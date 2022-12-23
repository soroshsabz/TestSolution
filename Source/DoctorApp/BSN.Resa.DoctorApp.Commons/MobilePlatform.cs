using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.DoctorApp.Commons
{
	public class MobilePlatform
	{
		private MobilePlatform(string mobilePlatformAsString)
		{
			_mobilePlatformAsString = mobilePlatformAsString.ToLower();
		}

		#region Public Methods

		public override string ToString()
		{
			return _mobilePlatformAsString;
		}

		public override bool Equals(object other)
		{
			if (!(other is MobilePlatform))
				return false;

			return _mobilePlatformAsString == ((MobilePlatform)other)._mobilePlatformAsString;
		}

		public override int GetHashCode()
		{
			return _mobilePlatformAsString != null ? _mobilePlatformAsString.GetHashCode() : 0;
		}

		#endregion

		#region Fields

		private readonly string _mobilePlatformAsString;

		#endregion

		#region Static Members

		public static bool operator ==(MobilePlatform firstMobilePlatform, MobilePlatform secondMobilePlatform)
		{
			return firstMobilePlatform.Equals(secondMobilePlatform);
		}

		public static bool operator !=(MobilePlatform firstMobilePlatform, MobilePlatform secondMobilePlatform)
		{
			return !firstMobilePlatform.Equals(secondMobilePlatform);
		}

		public static void TryParse(string mobilePlatformAsString, out MobilePlatform mobilePlatform)
		{
			mobilePlatformAsString = mobilePlatformAsString.ToLower();

			if (iOS.ToString() == mobilePlatformAsString)
				mobilePlatform = iOS;
			else if (Windows.ToString() == mobilePlatformAsString)
				mobilePlatform = Windows;
			else if (Android.ToString() == mobilePlatformAsString)
				mobilePlatform = Android;
			else
				mobilePlatform = null;
		}

		public static bool IsValidMobilePlatform(string mobilePlatformAsString)
		{
			MobilePlatform mobilePlatform;
			TryParse(mobilePlatformAsString, out mobilePlatform);

			return mobilePlatform != null;
		}

		public static MobilePlatform iOS => _iOS ?? (_iOS = new MobilePlatform("iOS"));
		private static MobilePlatform _iOS;

		public static MobilePlatform Windows => _windows ?? (_windows = new MobilePlatform("Windows"));
		private static MobilePlatform _windows;

		public static MobilePlatform Android => _android ?? (_android = new MobilePlatform("Android"));
		private static MobilePlatform _android;

		#endregion
	}

}
