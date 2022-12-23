namespace BSN.Resa.DoctorApp.Services
{
    public interface IDeviceInfo
    {
        bool IsDeviceXiaomiMiui();

        void OpenXiaomiMiuiAutostartSettingsPage();
    }
}