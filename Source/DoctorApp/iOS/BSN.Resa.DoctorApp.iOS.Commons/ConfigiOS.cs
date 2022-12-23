using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.iOS.Commons.Utilities;
using BSN.Resa.Locale;
using Foundation;
using System;

namespace BSN.Resa.DoctorApp.iOS.Commons
{
    public abstract class ConfigiOSBase : IConfigiOS
    {
        public string AppFullTitle => Resources.ResaFullName;

        public string ShareExtensionBundleIdentifier => "om.resaa.bsn.doctorapp.share-extension";

        public string CallDirectoryExtensionBundleIdentifier => "om.resaa.bsn.doctorapp.call-directory-extension";

        public string AppGroupIdentifier => "group.om.resaa.bsn.doctorapp";

        public string UrlScheme => "resa-doctorapp";

        public string ShareContactUrlIdentifier => "ShareContact";

        public MobilePlatform TargetPlatform => MobilePlatform.iOS;

        public string ResaMobileApiAddress => "https://resaa.net/api/Mobile/";

        public string ResaDoctorAppApiAddress => "https://resaa.net/api/DoctorApp/";

        public string ResaMobileApiToken => "5991848b-87ce-4646-a332-b7f2c7cfb221";

        public Version Version => Version.Parse((NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] as NSString).ToString()).Normalize();

        public abstract string ResaSmsReceiverPhoneNumber { get; }

        public abstract string AppCenterAppSecret { get; }

        public string ResaContactEmailAddress => "resaa.om@yahoo.com";

        public string ResaPrivacyPolicyUrl => "https://resaa.net/privacy/";

        public string AboutPageAboutDescription => Resources.DoctorAppAboutDescription;

        public string ResaContactPhoneNumber => "+982174471200";

        public string AboutPageResaContactMedium => ResaContactPhoneNumber;

        public Uri AboutPageResaContactMediumUri => new Uri($"tel:{ResaContactPhoneNumber}");

        public bool HasCallBlockingFeature => true;
    }
}