namespace BSN.Resa.Core.Commons.ViewModels
{
	public class AppUpdateManifest
	{
		public bool HasUrgentUpdate { get; set; }

		public bool HasNotifiableUpdate { get; set; }

		public AppUpdateInfo LatestDownloadableAppUpdateInfo { get; set; }
	}
}
