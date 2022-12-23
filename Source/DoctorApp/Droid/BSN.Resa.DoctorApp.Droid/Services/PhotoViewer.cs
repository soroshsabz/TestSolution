using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Utilities;
using Xamarin.Essentials;

namespace BSN.Resa.DoctorApp.Droid.Services
{
    public class PhotoViewer : IPhotoViewer
    {
        public async Task ShowPhoto(ReadOnlyFile photoFile)
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = photoFile
            });
        }
    }
}