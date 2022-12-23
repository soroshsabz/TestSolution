namespace BSN.Resa.Commons.Infrastructure
{
	public interface ICacheStatus
	{
		bool IsCacheEnabled { get; }
		bool IsCacheRunning { get; }
	}
}
