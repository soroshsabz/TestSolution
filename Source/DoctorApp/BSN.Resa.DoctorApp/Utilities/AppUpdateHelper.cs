using BSN.Resa.DoctorApp.Domain.Models;
using Plugin.Connectivity.Abstractions;

namespace BSN.Resa.DoctorApp.Utilities
{
    public interface IAppUpdateHelper
	{
		void ShowUpdateNotification();

		void StartUpdate(AppUpdate appUpdate, IConnectivity connectivity);

		bool HasOngoingUpdate();

        void PromptUpdateInstall();

        bool DoesDownloadedFileExist();

        void CancelNotification();
    }
}
