using BSN.Resa.DoctorApp.Services;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets.Utilities
{
    public class DeviceInfoiOS : IDeviceInfo
    {
        public bool IsDeviceXiaomiMiui()
        {
            return false;
        }

        public void OpenXiaomiMiuiAutostartSettingsPage()
        {
            throw new System.NotImplementedException();
        }
    }
}