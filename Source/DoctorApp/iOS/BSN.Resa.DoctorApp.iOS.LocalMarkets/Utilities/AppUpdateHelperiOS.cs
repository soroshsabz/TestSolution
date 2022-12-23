using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Utilities;
using Foundation;
using Plugin.Connectivity.Abstractions;
using UIKit;
using Xamarin.Forms;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets.Utilities
{
    public class AppUpdateHelperiOS : IAppUpdateHelper
	{
		public bool HasOngoingUpdate()
		{
			return false;
		}

        public void PromptUpdateInstall()
        {
        }

        public bool DoesDownloadedFileExist()
        {
            return false;
        }

        public void CancelNotification()
        {
           
        }

        public void ShowUpdateNotification()
		{
			
		}

		public void StartUpdate(AppUpdate appUpdate, IConnectivity connectivity)
		{
			if (!connectivity.IsConnected)
				throw new InternetNotAvailableException();

			string openingUrl = appUpdate.LatestDownloadableAppUpdateUrlLocally;

			if (openingUrl == null)
				return;

			Device.BeginInvokeOnMainThread(() =>
			{
				UIApplication.SharedApplication.OpenUrl(new NSUrl(openingUrl));
			});
		}
	}
}