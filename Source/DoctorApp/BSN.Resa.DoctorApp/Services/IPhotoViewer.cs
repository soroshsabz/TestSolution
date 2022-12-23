using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BSN.Resa.DoctorApp.Services
{
    public interface IPhotoViewer
    {
        Task ShowPhoto(ReadOnlyFile photoFile);
    }
}