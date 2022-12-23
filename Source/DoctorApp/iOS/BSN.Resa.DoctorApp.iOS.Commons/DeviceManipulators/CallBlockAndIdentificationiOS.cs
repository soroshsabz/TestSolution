using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using CallKit;

namespace BSN.Resa.DoctorApp.iOS.Commons.DeviceManipulators
{
	public class CallBlockAndIdentificationiOS: ICallBlockAndIdentification
	{
		public bool IsBlockingEnabled => IsBlockAndIdentificationEnabled;

		public bool IsIdentificationEnabled => IsBlockAndIdentificationEnabled;

		public void RefreshBlockedPhoneNumbers()
		{
			// Async approach waits infinitely on waiting
			CXCallDirectoryManager.SharedInstance.ReloadExtension(ConfigiOS.Instance.CallDirectoryExtensionBundleIdentifier, null);
		}

		private bool IsBlockAndIdentificationEnabled =>
				CXCallDirectoryManager.SharedInstance.GetEnabledStatusForExtensionAsync
				(ConfigiOS.Instance.CallDirectoryExtensionBundleIdentifier).Result 
					== CXCallDirectoryEnabledStatus.Enabled;
	}
}