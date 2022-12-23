using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;

namespace BSN.Resa.DoctorApp.iOS.Utilities
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