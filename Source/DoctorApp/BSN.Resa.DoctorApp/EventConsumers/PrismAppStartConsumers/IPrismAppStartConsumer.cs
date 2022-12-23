namespace BSN.Resa.DoctorApp.EventConsumers.PrismAppStartConsumers
{
	public interface IPrismAppStartConsumer : IEventConsumer
    {
		/// <summary>
		/// Calls on application start and returns navigation url.
		/// </summary>
		string OnStart();
    }
}
