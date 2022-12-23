using System;

namespace BSN.Resa.Commons.Infrastructure
{
	public interface ICache
	{
		void SetString(string key, string objectToCache, TimeSpan? expiry = null);
		void Set<T>(string key, T objectToCache, TimeSpan? expiry = null) where T : class;
		void Set<T>(T objectToCache) where T : class, ICacheEntity;
		void Delete(ICacheEntity objectToDelete);
		string GetString(string key);
		T Get<T>(string key) where T : class;
		void Delete(string key);
		void FlushAll();
	}
}
