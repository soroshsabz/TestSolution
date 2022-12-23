using System;

namespace BSN.Resa.DoctorApp.Commons
{
    public interface IConfig
    {
        string AppFullTitle { get; }

        MobilePlatform TargetPlatform { get; }

		string ResaMobileApiAddress { get; }

		string ResaMobileApiToken { get; }

		string ResaDoctorAppApiAddress { get; }

		string ResaSmsReceiverPhoneNumber { get; }

		string AppCenterAppSecret { get; }

		Version Version { get; }

		string ResaContactEmailAddress { get; }

		string ResaContactPhoneNumber { get; }

        string ResaPrivacyPolicyUrl { get; }

        string AboutPageAboutDescription { get; }

        string AboutPageResaContactMedium { get; }

        Uri AboutPageResaContactMediumUri { get; }

        bool HasCallBlockingFeature { get; }
    }
}