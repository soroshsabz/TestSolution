using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Foundation;

namespace BSN.Resa.DoctorApp.iOS.Commons.Utilities
{
	public interface IBlockedPhoneNumbers
	{
		List<long> GetAllSorted();

		void SetBlockedPhoneNumbers(List<long> blockedPhoneNumbers);
	}

	public class BlockedPhoneNumbers: IBlockedPhoneNumbers
	{
		public const string FileName = "CommaSeperatedBlockedPhoneNumbers";

		private static IBlockedPhoneNumbers _instance;

		public static IBlockedPhoneNumbers Instance
		{
			get { return _instance ?? (_instance = new BlockedPhoneNumbers()); }
			set { _instance = value; }
		}

		private readonly string _path;

		private BlockedPhoneNumbers()
		{
			string groupPath = NSFileManager.DefaultManager.GetContainerUrl(ConfigiOS.Instance.AppGroupIdentifier)?.Path;
			if (groupPath == null)
				throw new Exception($"App group \"{ConfigiOS.Instance.AppGroupIdentifier}\" doesn't exist.");

			_path = Path.Combine(groupPath, FileName);

			if (!File.Exists(_path))
				File.Create(_path).Close();
		}

		public List<long> GetAllSorted()
		{
			return new List<long>(File.ReadAllLines(_path).Select(long.Parse));
		}

		public void SetBlockedPhoneNumbers(List<long> blockedPhoneNumbers)
		{
			blockedPhoneNumbers.Sort();
			File.WriteAllText(_path, blockedPhoneNumbers.ToString(Environment.NewLine));
		}
	}
}
