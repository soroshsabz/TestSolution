using BSN.Resa.DoctorApp.EventConsumers;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.AppStartConsumers
{
	public interface IAppStartConsumer : IEventConsumer
	{
		void OnStart();
	}
}