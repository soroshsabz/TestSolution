namespace BSN.Resa.DoctorApp.EventConsumers.InternetAccessedConsumers
{
	public interface IInternetAccessedConsumer : IEventConsumer
	{
		void OnInternetAccessed();
	}
}
