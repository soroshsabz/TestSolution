using BSN.Resa.DoctorApp.Commons;

namespace BSN.Resa.DoctorApp.iOS.Commons
{
    public interface IConfigiOS : IConfig
    {
        string ShareExtensionBundleIdentifier { get; }

        string CallDirectoryExtensionBundleIdentifier { get; }

        string AppGroupIdentifier { get; }

        string UrlScheme { get; }

        string ShareContactUrlIdentifier { get; }
    }
}