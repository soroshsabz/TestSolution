using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Utilities;
using Plugin.Connectivity.Abstractions;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.AppUpdateNotificationClickedConsumers
{
	public class AppUpdateNotificationClickedConsumer : IAppUpdateNotificationClickedConsumer
	{
		public AppUpdateNotificationClickedConsumer(
			IAppUpdateRepository appUpdateRepository, 
			IAppUpdateHelper appUpdateHelper,
			IConnectivity connectivity)
		{
			_appUpdateRepository = appUpdateRepository;
			_appUpdateHelper = appUpdateHelper;
			_connectivity = connectivity;
		}

		public void OnClicked()
		{
			try
			{
				_appUpdateHelper.StartUpdate(_appUpdateRepository.Get(), _connectivity);
			}
			catch (ServiceCommunicationException)
			{
				// ignored
			}
		}

		private readonly IAppUpdateRepository _appUpdateRepository;

		private readonly IAppUpdateHelper _appUpdateHelper;

		private readonly IConnectivity _connectivity;
	}
}
