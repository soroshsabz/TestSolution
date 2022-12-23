using BSN.Resa.Core.Commons;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.iOS.Commons.Utilities;
using Foundation;
using System;
using BSN.Resa.Locale;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets
{
    public abstract class ConfigBase : IConfig
    {
        public string AppFullTitle => Resources.ResaFullName;

        public MobilePlatform TargetPlatform => MobilePlatform.iOS;

        public string ResaMobileApiAddress => "https://www.resaa.net/api/Mobile/";

        public string ResaDoctorAppApiAddress => "https://www.resaa.net/api/DoctorApp/";

        public string ResaMobileApiToken => "5991848b-87ce-4646-a332-b7f2c7cfb221";

        public string ResaSmsReceiverPhoneNumber => "10008000888808";

        public abstract string AppCenterAppSecret { get; }

        public Version Version => Version.Parse((NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] as NSString).ToString()).Normalize();

        public string ResaContactEmailAddress => "info@resaa.net";

        public string ResaContactPhoneNumber => "+982174471200";

        public string AboutPageAboutDescription => Resources.DoctorAppAboutDescription;

        public string ResaPrivacyPolicyUrl => "https://www.resaa.net/privacy";

        public string AboutPageResaContactMedium => ResaContactPhoneNumber;

        public Uri AboutPageResaContactMediumUri => new Uri($"tel:{ResaContactPhoneNumber}");

        public bool HasCallBlockingFeature => false;
    }
}