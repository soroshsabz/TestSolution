using BSN.Resa.DoctorApp.EventConsumers;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.AppUpdateNotificationClickedConsumers
{
	public interface IAppUpdateNotificationClickedConsumer : IEventConsumer
    {
		void OnClicked();
    }
}
