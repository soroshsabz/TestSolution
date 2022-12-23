using Android.App;
using Android.Content.PM;
using Android.Media;
using BSN.Resa.DoctorApp.Services;
using System;
using System.IO;
using Xamarin.Essentials;

namespace BSN.Resa.DoctorApp.Droid.Services
{
    public class VoiceRecorder : IVoiceRecorder
    {
        #region IVoiceRecorder

        public bool IsSupported()
        {
            PackageManager packageManager = Application.Context.PackageManager;
            bool isMicAvailable = packageManager.HasSystemFeature(PackageManager.FeatureMicrophone);

            return isMicAvailable;
        }

        public void StartRecording()
        {
            DeleteFileIfExists(VOICE_FILE_PATH);

            InitRecorder();

            ConfigRecorder();

            _recorder.Prepare();
            _recorder.Start();
        }

        public void StopRecording()
        {
            if (_recorder == null) return;

            _recorder.Stop();
            _recorder.Release();
            _recorder = null;
        }

        public string GetRecordedFilePath()
        {
            return VOICE_FILE_PATH;
        }

        public string GetVoiceFileNameWithExtension()
        {
            return VOICE_FILE_NAME_WITH_EXTENSION;
        }

        #endregion

        #region Private Methods

        private void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(VOICE_FILE_PATH))
            {
                File.Delete(VOICE_FILE_PATH);
            }
        }

        private void ConfigRecorder()
        {
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _recorder.SetOutputFile(VOICE_FILE_PATH);

            int maximumRecordingDuration = TimeSpan.FromMinutes(31).Milliseconds;
            _recorder.SetMaxDuration(maximumRecordingDuration);
        }

        private void InitRecorder()
        {
            if (_recorder == null)
                _recorder = new MediaRecorder();
            else
                _recorder.Reset();
        }

        #endregion

        #region Private Fields

        private const string VOICE_FILE_NAME_WITH_EXTENSION = "medical_test_voice_reply.aac";
        private static readonly string VOICE_FILE_PATH = Path.Combine(FileSystem.CacheDirectory, VOICE_FILE_NAME_WITH_EXTENSION);
        private MediaRecorder _recorder;

        #endregion
    }
}