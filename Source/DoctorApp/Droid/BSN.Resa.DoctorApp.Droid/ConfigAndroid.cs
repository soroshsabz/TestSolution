using Android.Content;
using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;
using System;
using BSN.Resa.Locale;

namespace BSN.Resa.DoctorApp.Droid
{
    public abstract class ConfigAndroidBase : IConfig
    {
        public string AppFullTitle => Resources.ResaFullName;

        public abstract MobilePlatform TargetPlatform { get; }

		public abstract string ResaMobileApiAddress { get; }

		public abstract string ResaMobileApiToken { get; }

		public abstract string ResaDoctorAppApiAddress { get; }

		public abstract string ResaSmsReceiverPhoneNumber { get; }

		public abstract string AppCenterAppSecret { get; }

        public abstract string ResaPrivacyPolicyUrl { get; }
        
        public Version Version => Version.Parse(Context.PackageManager.GetPackageInfo(Context.PackageName, 0).VersionName);

		private static Context Context => Android.App.Application.Context;

		public string ResaContactEmailAddress => "info@resaa.net";

		public string ResaContactPhoneNumber => "+982174471200";

        public string AboutPageAboutDescription => Resources.DoctorAppAboutDescription;

        public string AboutPageResaContactMedium => ResaContactPhoneNumber;

        public Uri AboutPageResaContactMediumUri => new Uri($"tel:{ResaContactPhoneNumber}");

        public bool HasCallBlockingFeature => true;
    }
}