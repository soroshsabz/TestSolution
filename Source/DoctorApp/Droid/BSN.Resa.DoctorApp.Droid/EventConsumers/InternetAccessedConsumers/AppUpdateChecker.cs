using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.EventConsumers.InternetAccessedConsumers;
using BSN.Resa.DoctorApp.Utilities;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.InternetAccessedConsumers
{
	public class AppUpdateChecker : IInternetAccessedConsumer
	{
		public AppUpdateChecker(
			IAppUpdateRepository appUpdateRepository,
			IUnitOfWork unitOfWork,
			IAppUpdateHelper appUpdateHelper,
			IConfig config)
		{
			_appUpdateRepository = appUpdateRepository;
			_unitOfWork = unitOfWork;
			_appUpdateHelper = appUpdateHelper;
			_config = config;
		}

		public async void OnInternetAccessed()
		{
			try
			{
				if (!_appUpdateHelper.HasOngoingUpdate() &&
						await _appUpdateRepository.Get().HasNotifiableUpdateAsync(_config.Version).ConfigureAwait(false))
					_appUpdateHelper.ShowUpdateNotification();

				_appUpdateRepository.Update(); //todo: vahid: isn't it better to move following two lines inside above if-block?
				_unitOfWork.Commit();
			}
			catch(ServiceCommunicationException)
			{
				// ignored
			}
		}

		private readonly IAppUpdateRepository _appUpdateRepository;

		private readonly IUnitOfWork _unitOfWork;

		private readonly IAppUpdateHelper _appUpdateHelper;

		private readonly IConfig _config;
	}
}
