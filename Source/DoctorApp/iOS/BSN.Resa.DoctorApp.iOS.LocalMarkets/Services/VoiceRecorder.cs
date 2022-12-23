using System;
using System.IO;
using AVFoundation;
using BSN.Resa.DoctorApp.Services;
using Foundation;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets.Services
{
    public class VoiceRecorder : IVoiceRecorder
    {
        #region IVoiceRecorder Methods

        public bool IsSupported()
        {
            return true;
        }

        public void StartRecording()
        {
            bool sessionResult = CreateSession();

            if(!sessionResult)
                return;

            ConfigRecorder();

            //Set Recorder to Prepare To Record
            bool prepareResult = _recorder.PrepareToRecord();

            if(!prepareResult)
                return;

            double maximumRecordingDuration = TimeSpan.FromMinutes(31).TotalSeconds;
            _recorder.RecordFor(maximumRecordingDuration);
        }

        public void StopRecording()
        {
            _recorder?.Stop();
        }

        public string GetRecordedFilePath()
        {
            return Path.Combine(Path.GetTempPath(), VOICE_FILE_NAME_WITH_EXTENSION);
        }

        public string GetVoiceFileNameWithExtension()
        {
            return VOICE_FILE_NAME_WITH_EXTENSION;
        }

        #endregion

        #region Private Methods

        private bool CreateSession()
        {
            var audioSession = AVAudioSession.SharedInstance();

            var error = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);

            if (error != null)
            {
                return false;
            }

            error = audioSession.SetActive(true);

            if (error != null)
            {
                return false;
            }

            return true;
        }

        private void ConfigRecorder()
        {
            _url = NSUrl.FromFilename(GetRecordedFilePath());
            //set up the NSObject Array of values that will be combined with the keys to make the NSDictionary
            NSObject[] values = new NSObject[]
            {
                NSNumber.FromFloat (16000.0f), //Sample Rate
                NSNumber.FromInt32 ((int)AudioToolbox.AudioFormatType.MPEG4AAC), //AVFormat
                NSNumber.FromInt32 (1), //Channels
                NSNumber.FromInt32 (16), //PCMBitDepth
                NSNumber.FromBoolean (false), //IsBigEndianKey
                NSNumber.FromBoolean (false) //IsFloatKey
            };

            //Set up the NSObject Array of keys that will be combined with the values to make the NSDictionary
            NSObject[] keys = new NSObject[]
            {
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVNumberOfChannelsKey,
                AVAudioSettings.AVLinearPCMBitDepthKey,
                AVAudioSettings.AVLinearPCMIsBigEndianKey,
                AVAudioSettings.AVLinearPCMIsFloatKey,
            };

            //Set Settings with the Values and Keys to create the NSDictionary
            _settings = NSDictionary.FromObjectsAndKeys(values, keys);

            //Set recorder parameters
            _recorder = AVAudioRecorder.Create(_url, new AudioSettings(_settings), out _error);
        }

        #endregion

        #region Private Fields

        private const string VOICE_FILE_NAME_WITH_EXTENSION = "medical_test_voice_reply.aac";
        private AVAudioRecorder _recorder;
        private NSError _error;
        private NSUrl _url;
        private NSDictionary _settings;

        #endregion
    }
}