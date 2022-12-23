using System;
using Newtonsoft.Json;
using NLog.Fluent;

namespace BSN.Resa.Commons.Infrastructure
{
	public abstract class Cache : ICache, ICacheStatus
	{
		private readonly bool _isCacheEnable;

		private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.All
        };

		protected Cache(bool isCacheEnable)
		{
			_isCacheEnable = isCacheEnable;
		}

		public void Set<T>(string key, T objectToCache, TimeSpan? expiry = null) where T : class
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_isCacheEnable)
			{
				try
				{
					var serializedObjectToCache = JsonConvert.SerializeObject(objectToCache, Formatting.None, _jsonSerializerSettings);

					SetStringProtected(key, serializedObjectToCache, expiry);
				}
				catch (Exception e)
				{
					Log.Error($"Cannot Set {key}. ExceptionMessage: {e.Message}");
				}
			}
		}

		public T Get<T>(string key) where T : class
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_isCacheEnable)
			{
				try
				{
					var stringObject = GetStringProtected(key);
					if (stringObject == null)
					{
						return default(T);
					}
					else
					{
						var obj = JsonConvert.DeserializeObject<T>(stringObject, _jsonSerializerSettings);

						return obj;
					}
				}
				catch (Exception e)
				{
					Log.Error($"Cannot Set key {key}. ExceptionMessage: {e.Message}");
				}
			}
			return null;
		}

		public void Delete(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_isCacheEnable)
			{
				try
				{
					DeleteProtected(key);
				}
				catch (Exception e)
				{
					Log.Error($"Cannot Delete key {key}. ExceptionMessage: {e.Message}");
				}
			}
		}

		public void FlushAll()
		{
			if (_isCacheEnable)
			{
				try
				{
					FlushAllProtected();
				}
				catch (Exception e)
				{
					Log.Error($"Cannot Flush. ExceptionMessage: {e.Message}");
				}
			}
		}

		public void Delete(ICacheEntity objectToDelete)
		{
			Delete(objectToDelete.CacheId);
		}

		public string GetString(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_isCacheEnable)
			{
				try
				{
					return GetStringProtected(key);
				}
				catch (Exception e)
				{
					Log.Error($"Cannot Set key {key}. ExceptionMessage: {e.Message}");
				}
			}
			return null;
		}

		public void SetString(string key, string objectToCache, TimeSpan? expiry = null)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_isCacheEnable)
			{
				try
				{
					SetStringProtected(key, objectToCache, expiry);
				}
				catch (Exception e)
				{
					Log.Error($"Cannot Set {key}. ExceptionMessage: {e.Message}");
				}
			}
		}

		public void Set<T>(T objectToCache) where T : class, ICacheEntity
		{
			Set(objectToCache.CacheId, objectToCache, objectToCache.CacheExpiry);
		}
		public bool IsCacheEnabled => _isCacheEnable;

		protected abstract void SetStringProtected(string key, string objectToCache, TimeSpan? expiry = null);
		protected abstract string GetStringProtected(string key);
		protected abstract void DeleteProtected(string key);
		protected abstract void FlushAllProtected();
		public abstract bool IsCacheRunning { get; }
	}
}