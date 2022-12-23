using Android.App;
using Android.Content;
using Android.Database;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Droid.Activities;
using BSN.Resa.DoctorApp.Utilities;
using Java.IO;
using Plugin.Connectivity.Abstractions;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace BSN.Resa.DoctorApp.Droid.Utilities
{
    public class AppUpdateHelperAndroid : IAppUpdateHelper
    {
        public const string DownloadsNotificationChannelId = "Notification.DownloadsChannel";
        public const int DownloadsNotificationId = 654123; //random number

        public void ShowUpdateNotification()
        {
            var intent = new Intent(Context, typeof(AppUpdateNotificationClickedActivity));

            PendingIntent pendingIntent = PendingIntent.GetActivity(Context, 0, intent, PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements
            var builder = new NotificationCompat.Builder(Context, DownloadsNotificationChannelId)
                .SetContentTitle(AppUpdateTitle)
                .SetContentText(Locale.Resources.DoctorAppClickToDownloadResaNewVersion)
                .SetSmallIcon(Resource.Drawable.resa_white_logo_small)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true);

            // Build the notification
            Notification notification = builder.Build();

            // Get the notification manager
            var notificationManager = (NotificationManager)Context.GetSystemService(Context.NotificationService);

            notificationManager.Notify(AppUpdateNotificationClickedActivity.NotificationId, notification);
        }

        public void StartUpdate(AppUpdate appUpdate, IConnectivity connectivity)
        {
            if (!connectivity.IsConnected)
                throw new InternetNotAvailableException();

            if (HasOngoingUpdate())
                return;

            try
            {
                string downloadingUrl = appUpdate.LatestDownloadableAppUpdateUrlLocally;
                Uri downloadingUri = Uri.Parse(downloadingUrl);
                
                Uri destinationUri = Uri.Parse($"file://{Destination}");

                var file = new File(Destination);

                if (file.Exists())
                    file.Delete();

                var request = new DownloadManager.Request(downloadingUri);
                request.SetAllowedOverMetered(true);
                request.SetTitle(AppUpdateTitle);
                request.SetDescription(AppUpdateTitle);
                request.SetDestinationUri(destinationUri);

                GetDownloadManager().Enqueue(request);

                DoctorAppSettings.IsUrgentUpdateDownloaded = false;
            }
            catch
            {
                //Ignored
            }

        }

        public bool HasOngoingUpdate()
        {
            var query = new DownloadManager.Query();
            query.SetFilterByStatus(DownloadStatus.Running);
            ICursor cursor = GetDownloadManager().InvokeQuery(query);
            while (cursor.MoveToNext())
            {
                string title = cursor.GetString(cursor.GetColumnIndex(DownloadManager.ColumnTitle));
                if (title == AppUpdateTitle)
                    return true;
            }
            return false;
        }

        public void PromptUpdateInstall()
        {
            var install = new Intent(Intent.ActionView);
            install.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                File downloadedFile = new File($"{Context.GetExternalFilesDir(Environment.DirectoryDownloads)}/resa-doctor.apk");

                Uri downloadedFileUri = FileProvider.GetUriForFile(Context, "ResaDoctorApp.Droid.fileprovider", downloadedFile);

                install.SetDataAndType(downloadedFileUri, "application/vnd.android.package-archive");
                install.AddFlags(ActivityFlags.GrantReadUriPermission);
            }
            else
            {
                install.SetDataAndType(Uri.Parse(GetDownloadUri()), "application/vnd.android.package-archive");
            }

            Context.StartActivity(install);
        }

        public bool DoesDownloadedFileExist()
        {
            return System.IO.File.Exists(Destination);
        }

        public void CancelNotification()
        {
            NotificationManager notificationManager = Context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Cancel(DownloadsNotificationId);
        }

        #region Private Methods

        private string GetDownloadUri()
        {
            var query = new DownloadManager.Query();

            query.SetFilterById(DoctorAppSettings.UrgentUpdateDownloadId);

            ICursor cursor = GetDownloadManager().InvokeQuery(query);

            if (cursor.MoveToFirst() && cursor.Count > 0)
            {
                var status = (DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus));
                if (status != DownloadStatus.Successful)
                    return null;
            }

            string localUri = cursor.GetString(cursor.GetColumnIndex(DownloadManager.ColumnLocalUri));

            cursor.Close();

            return localUri;
        }

        private DownloadManager GetDownloadManager()
        {
            var downloadManager = (DownloadManager)Context.GetSystemService(Context.DownloadService);
            return downloadManager;
        }

        #endregion

        #region Private Fields

        private static readonly string AppUpdateTitle = Locale.Resources.UpdatingResa;
        private static readonly string Destination = $"{Context.GetExternalFilesDir(Environment.DirectoryDownloads)}/resa-doctor.apk";
        private static Context Context => Application.Context;

        #endregion
    }
}