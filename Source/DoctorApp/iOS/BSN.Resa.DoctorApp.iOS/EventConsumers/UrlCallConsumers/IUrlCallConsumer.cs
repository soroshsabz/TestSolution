using BSN.Resa.DoctorApp.EventConsumers;
using Foundation;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.UrlCallConsumers
{
	public interface IUrlCallConsumer : IEventConsumer
	{
		bool OnUrlCall(NSUrl url);
	}
}