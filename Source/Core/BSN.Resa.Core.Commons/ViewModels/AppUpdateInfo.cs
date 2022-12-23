using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
	public class AppUpdateInfoDataAccessObject
	{
		public string Type { get; set; }

		public string Url { get; set; }

		public string Version { get; set; }
	}

	public class AppUpdateInfo: IComparable<AppUpdateInfo>
	{
		#region Constructors

		// For serialization/deserialization use
		public AppUpdateInfo() { }

		public AppUpdateInfo(AppUpdateInfoDataAccessObject appUpdateInfoDataAccessObject, IAppUpdateFactory appUpdateFactory)
		{
			Url = appUpdateInfoDataAccessObject.Url;

			Version.TryParse(appUpdateInfoDataAccessObject.Version, out var version);
			Version = version ?? throw new ArgumentException(nameof(appUpdateInfoDataAccessObject.Version));

			Type = appUpdateFactory.Get(appUpdateInfoDataAccessObject.Type);
		}

		public AppUpdateInfo(AppUpdateInfoDataAccessObject appUpdateInfoDataAccessObject)
			: this(appUpdateInfoDataAccessObject, AppUpdateFactory.Instance)
		{ }

		#endregion

		#region Public Methods

		public int CompareTo(AppUpdateInfo other)
		{
			return Version.CompareTo(other.Version);
		}

		#endregion

		#region Properties

		public AppUpdateType Type { get; set; }

		public Version Version { get; set; }

		public string Url { get; set; }

		#endregion
	}

	public class AppUpdateType
	{
		public virtual bool IsDownloadable { get; set; }

		public virtual bool IsNotifiable { get; set; }

		public virtual bool IsUrgent { get; set; }
	}

	public interface IAppUpdateFactory
	{
		AppUpdateType Get(string appUpdateType);
	}

	public class AppUpdateFactory: IAppUpdateFactory
	{
		#region Singleton

		private static AppUpdateFactory _instance;

		public static AppUpdateFactory Instance => _instance ?? (_instance = new AppUpdateFactory());

		#endregion

		private AppUpdateFactory()
		{ }

		public AppUpdateType Get(string appUpdateType)
		{
			switch (appUpdateType.ToLower())
			{
				case "default":
					return new DefaultAppUpdate();
				case "urgent":
					return new UrgentAppUpdate();
				case "notify":
					return new NotifyAppUpdate();
				case "internal":
					return new InternalAppUpdate();
				default:
					return new DefaultAppUpdate();
			}
		}
	}

	public class InternalAppUpdate: AppUpdateType
	{
		public override bool IsDownloadable => false;

		public override bool IsNotifiable => false;

		public override bool IsUrgent => false;
	}

	public class DefaultAppUpdate: AppUpdateType
	{
		public override bool IsDownloadable => true;

		public override bool IsNotifiable => false;

		public override bool IsUrgent => false;
	}

	public class NotifyAppUpdate : DefaultAppUpdate
	{
		public override bool IsNotifiable => true;
	}

	public class UrgentAppUpdate : NotifyAppUpdate
	{
		public override bool IsUrgent => true;
	}
}

