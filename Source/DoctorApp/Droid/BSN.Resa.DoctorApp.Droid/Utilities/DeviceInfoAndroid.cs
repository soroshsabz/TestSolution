using Android.Content;
using Android.OS;
using Android.Text;
using BSN.Resa.DoctorApp.Services;
using Java.IO;
using Java.Lang;
using Application = Android.App.Application;

namespace BSN.Resa.DoctorApp.Droid.Utilities
{
    public class DeviceInfoAndroid : IDeviceInfo
    {
        #region Constructor

        public DeviceInfoAndroid()
        {
            _context = Application.Context;
        }

        #endregion

        #region IDeviceInfo Methods

        public bool IsDeviceXiaomiMiui()
        {
            var isXiaomi = DeviceManufacturer() == "xiaomi" || DeviceManufacturer() == "redmi";
            return isXiaomi && IsMiUi();
        }

        public void OpenXiaomiMiuiAutostartSettingsPage()
        {
            Intent intent = new Intent();
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.NoHistory);
            intent.SetComponent(new ComponentName("com.miui.securitycenter",
                "com.miui.permcenter.autostart.AutoStartManagementActivity"));
            try
            {
                _context.StartActivity(intent);
            }
            catch (Exception)
            {
                //Ignored
            }
        }

        #endregion

        #region Private Methods

        private static string GetSystemProperty(string propName)
        {
            string line;
            BufferedReader input = null;
            try
            {
                Java.Lang.Process p = Runtime.GetRuntime().Exec("getprop " + propName);
                input = new BufferedReader(new InputStreamReader(p.InputStream), 1024);
                line = input.ReadLine();
                input.Close();
            }
            catch (IOException)
            {
                return null;
            }
            finally
            {
                if (input != null)
                {
                    try
                    {
                        input.Close();
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }

            return line;
        }

        private string DeviceManufacturer()
        {
            return Build.Manufacturer?.ToLowerInvariant();
        }

        private static bool IsMiUi()
        {
            return !TextUtils.IsEmpty(GetSystemProperty("ro.miui.ui.version.name"));
        }

        #endregion

        #region Private Fields

        private readonly Context _context;

        #endregion
    }
}