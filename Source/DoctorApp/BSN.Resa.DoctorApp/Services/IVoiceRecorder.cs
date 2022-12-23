namespace BSN.Resa.DoctorApp.Services
{
    public interface IVoiceRecorder
    {
        bool IsSupported();

        void StartRecording();

        void StopRecording();

        string GetRecordedFilePath();

        string GetVoiceFileNameWithExtension();
    }
}