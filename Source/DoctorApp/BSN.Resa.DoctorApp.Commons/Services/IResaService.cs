namespace BSN.Resa.DoctorApp.Commons.Services
{
    public interface IResaService
    {
        bool Start();
        bool Stop();
        bool CanStart();
    }
}