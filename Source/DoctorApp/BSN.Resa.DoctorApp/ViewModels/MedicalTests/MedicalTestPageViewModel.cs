using BSN.Resa.DoctorApp.Aspects;
using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Data.ServiceCommunicators;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using BSN.Resa.DoctorApp.Views.Utilities;
using BSN.Resa.Locale;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using Plugin.Permissions.Abstractions;
using Plugin.SimpleAudioPlayer;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace BSN.Resa.DoctorApp.ViewModels.MedicalTests
{
    public class MedicalTestPageViewModel : AbstractMedicalTestPageViewModel
    {
        #region Constructor

        public MedicalTestPageViewModel(
            INavigationService navigationService,
            IConnectivity connectivity,
            IGsmConnection gsmConnection,
            IPageDialogService pageDialogService,
            ISmsTask smsTask,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            ICrashReporter crashReporter,
            ICallbackRequestRepository callbackRequestRepository,
            IPermissionsManager permissionsManager,
            ConnectionStatusManager connectionStatusManager,
            IPhotoViewer photoViewer,
            IPermissions permissions,
            IVoiceRecorder voiceRecorder,
            ISimpleAudioPlayer simpleAudioPlayer,
            IMedicalTestRepository medicalTestRepository,
            IConfig config) : base(navigationService, connectivity, gsmConnection,
            pageDialogService, smsTask, doctorRepository, medicalTestRepository, unitOfWork, crashReporter, callbackRequestRepository,
            permissionsManager, connectionStatusManager)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;
            _photoViewer = photoViewer;
            _permissions = permissions;
            _voiceRecorder = voiceRecorder;
            _simpleAudioPlayer = simpleAudioPlayer;
            _config = config;

            _recordingStopwatch = new Stopwatch();
            _recordingTimer = new Timer(OnRecordingTimerElapsed, null, dueTime: -1, period: TIMER_INTERVAL);

            _playingStopwatch = new Stopwatch();
            _playingTimer = new Timer(OnPlayingTimerElapsed, null, dueTime: -1, period: TIMER_INTERVAL);

            DisableVoiceRecordingIfDeviceNotSupports();
        }

        #endregion

        #region Life-cycle Methods

        public override void OnAppearing()
        {
            base.OnAppearing();

            RegisterEvents();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            StopRecordingIfIsRecording();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            bool isMedicalTestAvailable = await TryLoadMedicalTest(parameters);

            if (!isMedicalTestAvailable) return;

            LoadMedicalTestPhotos();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            DisposeTimers();

            CloseAndDisposeVoiceFile();

            DeleteVoiceFileIfExists();

            CancelAndDisposeCancellationToken();

            UnRegisterEvents();
        }

        #endregion

        #region Bound Properties

        public MedicalTest MedicalTest
        {
            get => _medicalTest;
            set => SetProperty(ref _medicalTest, value);
        }

        public ObservableCollection<string> MedicalTestPhotos
        {
            get => _medicalTestPhotos;
            set => SetProperty(ref _medicalTestPhotos, value);
        }

        public ICommand OnPhotoSelected => new Command<string>(async (photoUrl) =>
        {
            if (_isRecording || photoUrl.IsNullOrEmptyOrSpace())
                return;

            await ShowFullPhoto(photoUrl);
        });

        public ICommand StartRecordVoiceCommand => new Command(async () =>
        {
            bool isGranted = await EnsureRecordPermissionIsGranted();

            if (!isGranted)
            {
                await _pageDialogService.DisplayAlertAsync("", Resources.DoctorAppEnableMicPermissionToRecordVoice,
                    Resources.Ok);
                return;
            }

            await StartRecordingVoice();
        });

        public ICommand RemoveRecordedVoiceCommand => new Command(async () =>
        {
            var choice = await _pageDialogService.DisplayAlertAsync("", Resources.DoctorAppMedicalTestRemoveRecordedVoiceMessage, Resources.Ok, Resources.Cancel);

            if (!choice)
                return;

            CloseAndDisposeVoiceFile();

            DeleteVoiceFileIfExists();

            IsRecordingElapsedTimeEnable = false;
            IsPlayingElapsedTimeEnable = false;

            ResetTrackingPlayTime();

            IsPlayVoiceEnable = false;
            IsPauseVoiceEnable = false;

            IsStopRecordingEnable = false;

            IsPlayPauseRemoveEnable = false;

            IsStartRecordingEnable = true;

            UpdateSubmitReplyButtonState();
        });

        public ICommand StopRecordVoiceCommand => new Command(StopRecordingVoice);

        public bool IsStartRecordingEnable
        {
            get => _isStartRecordingEnable;
            set => SetProperty(ref _isStartRecordingEnable, value);
        }

        public bool IsStopRecordingEnable
        {
            get => _isStopRecordingEnable;
            set => SetProperty(ref _isStopRecordingEnable, value);
        }

        public TimeSpan RecordingElapsedTime
        {
            get => _recordingElapsedTime;
            set => SetProperty(ref _recordingElapsedTime, value);
        }

        public TimeSpan PlayingElapsedTime
        {
            get => _playingElapsedTime;
            set => SetProperty(ref _playingElapsedTime, value);
        }

        public bool IsRecordingElapsedTimeEnable
        {
            get => _isRecordingElapsedTimeEnable;
            set => SetProperty(ref _isRecordingElapsedTimeEnable, value);
        }

        public bool IsPlayingElapsedTimeEnable
        {
            get => _isPlayingElapsedTimeEnable;
            set => SetProperty(ref _isPlayingElapsedTimeEnable, value);
        }

        public ICommand SubmitMedicalTestReplyCommand => new Command(async () => { await SubmitRepliesToServer(); });

        public bool IsPlayPauseRemoveEnable
        {
            get => _isPlayPauseRemoveEnable;
            set => SetProperty(ref _isPlayPauseRemoveEnable, value);
        }

        public bool SubmitMedicalTestReplyButtonEnable
        {
            get => _submitMedicalTestReplyButtonEnable;
            set => SetProperty(ref _submitMedicalTestReplyButtonEnable, value);
        }

        public string MedicalTestTextReply
        {
            get => _medicalTestTextReply;
            set => SetProperty(ref _medicalTestTextReply, value);
        }

        public ICommand OnTextReplyCompletedCommand => new Command(() =>
       {
           if (!MedicalTestTextReply.IsNullOrEmptyOrSpace())
           {
               MedicalTestTextReply = MedicalTestTextReply.Trim();
           }

           if (!_isRecording)
           {
               UpdateSubmitReplyButtonState();
           }
       });

        public bool IsPlayVoiceEnable
        {
            get => _isPlayVoiceEnable;
            set => SetProperty(ref _isPlayVoiceEnable, value);
        }

        public bool IsPauseVoiceEnable
        {
            get => _isPauseVoiceEnable;
            set => SetProperty(ref _isPauseVoiceEnable, value);
        }

        public ICommand PlayVoiceCommand => new Command(() =>
        {
            IsPlayVoiceEnable = false;
            IsPauseVoiceEnable = true;

            IsRecordingElapsedTimeEnable = false;
            IsPlayingElapsedTimeEnable = true;

            try
            {
                if (!_isVoiceFileOpen)
                {
                    LoadVoiceFileStream();
                    _simpleAudioPlayer.Load(_voiceFileStream);

                    _isVoiceFileOpen = true;
                }

                _simpleAudioPlayer.Play();

                StartTrackingPlayTime();
            }
            catch (Exception exception)
            {
                var contextInformation = new Dictionary<string, string>
                {
                    {"MedicalTest", "Playing recorded voice"}
                };

                CrashReporter.SendException(exception, contextInformation);
            }
        });

        public ICommand PauseVoiceCommand => new Command(() =>
       {
           IsRecordingElapsedTimeEnable = false;
           IsPlayingElapsedTimeEnable = true;

           IsPlayVoiceEnable = true;
           IsPauseVoiceEnable = false;

           _simpleAudioPlayer.Pause();

           StopTrackingPlayTime();
       });

        public bool IsSubmittingReplyDialogVisible
        {
            get => _isSubmittingReplyDialogVisible;
            set => SetProperty(ref _isSubmittingReplyDialogVisible, value);
        }

        public ICommand OnSubmittingReplyDialogCancelCommand => new Command(() =>
            {
                _cancellationTokenSource?.Cancel();
                IsSubmittingReplyDialogVisible = false;
            });

        public bool IsSuccessfulReplySubmitViewVisible
        {
            get => _isSuccessfulReplySubmitViewVisible;
            set => SetProperty(ref _isSuccessfulReplySubmitViewVisible, value);
        }

        public ICommand ShowNextMedicalTestCommand => new Command(async () =>
        {
            IsSuccessfulReplySubmitViewVisible = false;

            DeleteCachedPhotos();

            await LoadNextMedicalTest();
        });

        #endregion

        #region Private Methods

        private async Task<bool> TryLoadMedicalTest(INavigationParameters parameters)
        {
            string medicalTestId = parameters["SelectedMedicalTestId"] as string;

            if (medicalTestId.IsNullOrEmptyOrSpace())
            {
                await _navigationService.GoBackAsync();

                return false;
            }

            var medicalTest = MedicalTestRepository.Get(medicalTestId);

            if (medicalTest == null)
            {
                await _navigationService.GoBackAsync();

                return false;
            }

            MedicalTest = medicalTest;

            return true;
        }

        private void LoadMedicalTestPhotos()
        {
            //remove the ending '/' character
            string baseUrl = _config.ResaMobileApiAddress.TrimEnd('/');

            MedicalTestPhotos = MedicalTest.Photos?.Select(relativeUrl => $"{baseUrl}{relativeUrl}").ToObservableCollection();
        }

        private void DisableVoiceRecordingIfDeviceNotSupports()
        {
            IsStartRecordingEnable = _voiceRecorder.IsSupported();
        }

        private async Task StartRecordingVoice()
        {
            CloseAndDisposeVoiceFile();

            IsPlayingElapsedTimeEnable = false;
            IsRecordingElapsedTimeEnable = true;

            ResetTrackingPlayTime();

            ResetTrackingRecordTime();

            IsStartRecordingEnable = false;
            IsPlayPauseRemoveEnable = false;
            IsStopRecordingEnable = true;

            StartTrackingRecordTime();

            SubmitMedicalTestReplyButtonEnable = false;

            try
            {
                _voiceRecorder.StartRecording();
                _isRecording = true;
            }
            catch (Exception exception)
            {
                _isRecording = false;

                await ShowVoiceRecordingErrorMessage();

                var contextInformation = new Dictionary<string, string>
                {
                    {"MedicalTest", "Recording voice"}
                };

                CrashReporter.SendException(exception, contextInformation);
            }
        }

        private void StopRecordingVoice()
        {
            _isRecording = false;

            try
            {
                _voiceRecorder.StopRecording();
            }
            catch
            {
                // ignored
            }

            StopTrackingRecordTime();

            IsStartRecordingEnable = false;
            IsPlayPauseRemoveEnable = true;
            IsPlayVoiceEnable = true;
            IsPauseVoiceEnable = false;
            IsStopRecordingEnable = false;
            IsRecordingElapsedTimeEnable = true;

            SubmitMedicalTestReplyButtonEnable = true;
        }

        private void OnRecordingTimerElapsed(object arg)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                StopRecordingIfReachedTimeLimit();

                RecordingElapsedTime = _recordingStopwatch.Elapsed;
            });
        }

        private void StopRecordingIfReachedTimeLimit()
        {
            var timeLimit = TimeSpan.FromMinutes(30);
            if (_recordingStopwatch.Elapsed >= timeLimit)
            {
                StopRecordingVoice();

                Device.InvokeOnMainThreadAsync(async () => await AlertDoctorWhenReachedRecordingTimeLimit());
            }
        }

        private void OnPlayingTimerElapsed(object arg)
        {
            Device.InvokeOnMainThreadAsync(() =>
            {
                {
                    PlayingElapsedTime = _playingStopwatch.Elapsed;
                }
            });
        }

        private async Task ShowFullPhoto(string photoUrl)
        {
            string photoCachedKey = ImageService.Instance.Config.MD5Helper.MD5(photoUrl);

            string fullCachedPhotoPath = await ImageService.Instance.Config.DiskCache.GetFilePathAsync(photoCachedKey);

            using (Stream streamPhoto = await ImageService.Instance.LoadFile(fullCachedPhotoPath).AsPNGStreamAsync())
            {
                string fullSavedPhoto = Path.Combine(FileSystem.CacheDirectory, "medical_test_photo_to_show.png");

                File.WriteAllBytes(fullSavedPhoto, streamPhoto.ToByteArray());

                await _photoViewer.ShowPhoto(new ReadOnlyFile(fullSavedPhoto));
            }
        }

        private async Task<bool> EnsureRecordPermissionIsGranted()
        {
            var result = await _permissions.RequestPermissionsAsync(Permission.Microphone);
            bool isGranted = result[Permission.Microphone] == PermissionStatus.Granted;
            return isGranted;
        }

        private void StartTrackingRecordTime()
        {
            _recordingTimer.Change(dueTime: 0, TIMER_INTERVAL);
            _recordingStopwatch.Start();
        }

        private void StartTrackingPlayTime()
        {
            _playingTimer.Change(dueTime: 0, TIMER_INTERVAL);
            _playingStopwatch.Start();
        }

        private void StopTrackingRecordTime()
        {
            _recordingTimer.Change(dueTime: -1, period: -1);
            _recordingStopwatch.Stop();
        }

        private void StopTrackingPlayTime()
        {
            _playingTimer.Change(dueTime: -1, period: -1);
            _playingStopwatch.Stop();
        }

        private void ResetTrackingRecordTime()
        {
            _recordingStopwatch.Reset();

            RecordingElapsedTime = TimeSpan.Zero;
        }

        private void ResetTrackingPlayTime()
        {
            _playingStopwatch.Reset();
        }

        private void OnPlayingVoiceEnded(object sender, EventArgs e)
        {
            IsRecordingElapsedTimeEnable = true;
            IsPlayingElapsedTimeEnable = false;

            IsPlayVoiceEnable = true;
            IsPauseVoiceEnable = false;

            ResetTrackingPlayTime();
        }

        private void CloseAndDisposeVoiceFile()
        {
            _isVoiceFileOpen = false;

            try
            {
                StopPlayingIfIsPlaying();

                _voiceFileStream?.Close();
                _voiceFileStream?.Dispose();
                _voiceFileStream = null;
            }
            catch
            {
                // ignored
            }
        }

        private void DeleteVoiceFileIfExists()
        {
            if (File.Exists(_voiceRecorder.GetRecordedFilePath()))
            {
                try
                {
                    File.Delete(_voiceRecorder.GetRecordedFilePath());
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void StopRecordingIfIsRecording()
        {
            if (_isRecording)
            {
                StopRecordingVoice();
            }
        }

        private void StopPlayingIfIsPlaying()
        {
            if (_simpleAudioPlayer.IsPlaying)
            {
                _simpleAudioPlayer.Stop();
            }
        }

        private async Task AlertDoctorWhenReachedRecordingTimeLimit()
        {
            await _pageDialogService.DisplayAlertAsync("", Resources.DoctorAppReachedVoiceRecordingTimeLimitMessage, Resources.Ok);
        }

        private void RegisterEvents()
        {
            _simpleAudioPlayer.PlaybackEnded += OnPlayingVoiceEnded;
        }

        private void UnRegisterEvents()
        {
            _simpleAudioPlayer.PlaybackEnded -= OnPlayingVoiceEnded;
        }

        private void DisposeTimers()
        {
            try
            {
                _playingTimer?.Dispose();

                _recordingTimer?.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        private async Task ShowVoiceRecordingErrorMessage()
        {
            await _pageDialogService.DisplayAlertAsync("", Resources.DoctorAppVoiceRecordingErrorMessage, Resources.Ok);
        }

        private void UpdateSubmitReplyButtonState()
        {
            SubmitMedicalTestReplyButtonEnable = IsTextReplyAvailable() || IsVoiceReplyAvailable();
        }

        private bool IsTextReplyAvailable()
        {
            return !MedicalTestTextReply.IsNullOrEmptyOrSpace();
        }

        private bool IsVoiceReplyAvailable()
        {
            return File.Exists(_voiceRecorder.GetRecordedFilePath());
        }

        private void LoadVoiceFileStream()
        {
            _voiceFileStream = new FileStream(_voiceRecorder.GetRecordedFilePath(), FileMode.Open);
        }

        [CentralizedExceptionHandler]
        private async Task SubmitRepliesToServer()
        {
            if (!_connectivity.IsConnected)
            {
                await ShowInternetNotAvailableAlertAsync();

                return;
            }

            InitCancellationToken();

            IsSubmittingReplyDialogVisible = true;

            var replyTasks = new List<Task>();

            if (IsTextReplyAvailable())
            {
                MedicalTest.TextReply = MedicalTestTextReply;

                replyTasks.Add(MedicalTest.SubmitTextReply(_taskCancellationToken));
            }

            if (IsVoiceReplyAvailable())
            {
                if (_voiceFileStream == null)
                {
                    LoadVoiceFileStream();
                }

                _voiceFileStream.Position = 0;

                MedicalTest.VoiceFileStream = _voiceFileStream;
                replyTasks.Add(MedicalTest.SubmitVoiceReply(_voiceRecorder.GetVoiceFileNameWithExtension(), _taskCancellationToken));
            }

            try
            {
                await Task.WhenAll(replyTasks);

                var currentMedicalTestInDb = MedicalTestRepository.Get(MedicalTest.Id);

                MedicalTestRepository.Delete(currentMedicalTestInDb);

                UnitOfWork.Commit();

                IsSubmittingReplyDialogVisible = false;

                IsSuccessfulReplySubmitViewVisible = true;

                CloseAndDisposeVoiceFile();

                try
                {
                    await SyncDbMedicalTestsWithServer(_taskCancellationToken);
                }
                catch
                {
                    // ignored
                }
            }
            catch (OperationCanceledException)
            {
                //Ignored
            }
            catch (NetworkConnectionException networkConnectionException)
            {
                if (!networkConnectionException.IsOperationCancelled)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsSubmittingReplyDialogVisible = false;
            }
        }

        private void InitCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _taskCancellationToken = _cancellationTokenSource.Token;
        }

        private async Task LoadNextMedicalTest()
        {
            var nextMedicalTest = MedicalTestRepository.GetAll()?.FirstOrDefault(medicalTest => !medicalTest.Id.Equals(MedicalTest.Id));

            if (nextMedicalTest == null)
            {
                await _navigationService.GoBackAsync();

                return;
            }

            MedicalTest = nextMedicalTest;

            ResetTrackingPlayTime();
            RecordingElapsedTime = TimeSpan.Zero;

            ResetTrackingRecordTime();

            IsPlayPauseRemoveEnable = false;

            IsStopRecordingEnable = false;

            IsStartRecordingEnable = true;

            IsPlayingElapsedTimeEnable = false;

            IsRecordingElapsedTimeEnable = false;

            MedicalTestTextReply = string.Empty;

            LoadMedicalTestPhotos();
        }

        private void DeleteCachedPhotos()
        {
            foreach (var medicalTestPhoto in MedicalTestPhotos)
            {
                CachedImage.InvalidateCache(medicalTestPhoto, CacheType.Disk);
            }
        }

        private void CancelAndDisposeCancellationToken()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        #endregion

        #region Private Fields

        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;
        private readonly IPhotoViewer _photoViewer;
        private readonly IPermissions _permissions;
        private readonly IVoiceRecorder _voiceRecorder;
        private readonly ISimpleAudioPlayer _simpleAudioPlayer;
        private readonly IConfig _config;
        private MedicalTest _medicalTest;
        private ObservableCollection<string> _medicalTestPhotos;
        private TimeSpan _recordingElapsedTime;
        private TimeSpan _playingElapsedTime;
        private bool _isStartRecordingEnable;
        private bool _isStopRecordingEnable;
        private bool _isRecordingElapsedTimeEnable;
        private readonly Stopwatch _recordingStopwatch;
        private readonly Stopwatch _playingStopwatch;
        private readonly Timer _recordingTimer;
        private readonly Timer _playingTimer;
        private bool _isPlayPauseRemoveEnable;
        private bool _submitMedicalTestReplyButtonEnable;
        private string _medicalTestTextReply;
        private bool _isPlayVoiceEnable;
        private bool _isPauseVoiceEnable;
        private bool _isPlayingElapsedTimeEnable;
        private bool _isRecording;
        private bool _isVoiceFileOpen;
        private FileStream _voiceFileStream;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _taskCancellationToken;
        private bool _isSubmittingReplyDialogVisible;
        private bool _isSuccessfulReplySubmitViewVisible;
        private const int TIMER_INTERVAL = 500; //timers' callback will be fired every 500 milliseconds

        #endregion
    }
}