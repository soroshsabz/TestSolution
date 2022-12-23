using System;
using Newtonsoft.Json;

namespace BSN.Resa.Commons.Infrastructure
{
	public interface ICacheEntity
	{
		[JsonIgnore]
		string CacheId { get; }
		[JsonIgnore]
		TimeSpan? CacheExpiry { get; }
	}
}