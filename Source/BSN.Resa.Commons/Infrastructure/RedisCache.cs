using System;
using NLog.Fluent;
using StackExchange.Redis;

namespace BSN.Resa.Commons.Infrastructure
{
	public class RedisCache : Cache
	{
		private ConnectionMultiplexer _redisConnections;

		private IDatabase RedisDatabase
		{
			get
			{
				if (_redisConnections == null)
				{
					InitializeConnection();
				}
				return _redisConnections?.GetDatabase();
			}
		}

		public RedisCache(bool isCacheEnabled) : base(isCacheEnabled)
		{
			InitializeConnection();
		}

		private void InitializeConnection()
		{
			try
			{
				_redisConnections = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.AppSettings["CacheConnectionString"]);
			}
			catch (RedisConnectionException errorConnectionException)
			{
				Log.Error($"Error connecting the redis cache : {errorConnectionException.Message}");
			}
		}

		protected override string GetStringProtected(string key)
		{
			if (RedisDatabase == null)
			{
				return null;
			}
			var redisObject = RedisDatabase.StringGet(key);
			return redisObject.HasValue ? redisObject.ToString() : null;
		}

		protected override void SetStringProtected(string key, string objectToCache, TimeSpan? expiry = null)
		{
			RedisDatabase?.StringSet(key, objectToCache, expiry);
		}

		protected override void DeleteProtected(string key)
		{
			RedisDatabase?.KeyDelete(key);
		}

		protected override void FlushAllProtected()
		{
			if (RedisDatabase == null)
			{
				return;
			}
			var endPoints = _redisConnections.GetEndPoints();
			foreach (var endPoint in endPoints)
			{
				var server = _redisConnections.GetServer(endPoint);
				server.FlushAllDatabases();
			}
		}

		public override bool IsCacheRunning => _redisConnections?.IsConnected ?? false;
	}
}