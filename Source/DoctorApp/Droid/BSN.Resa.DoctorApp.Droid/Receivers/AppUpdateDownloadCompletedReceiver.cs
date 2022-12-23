using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Widget;
using BSN.Resa.DoctorApp.Data;
using BSN.Resa.DoctorApp.Droid.Activities;
using BSN.Resa.DoctorApp.Droid.Utilities;

namespace BSN.Resa.DoctorApp.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { DownloadManager.ActionDownloadComplete })]
    public class AppUpdateDownloadCompletedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Checking owner
            if (context.PackageName != intent.Package) return;

            // Getting download id
            _downloadId = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);

            DoctorAppSettings.UrgentUpdateDownloadId = _downloadId;

            _context = context;

            // Check download status
            if (!IsSuccessful())
            {
                DoctorAppSettings.IsUrgentUpdateDownloaded = false;

                return;
            }

            DoctorAppSettings.IsUrgentUpdateDownloaded = true;

            // Toasting download successful
            Toast.MakeText(context, Locale.Resources.DoctorAppNewAppVersionDownloaded, ToastLength.Long).Show();

            ConfigAndShowNotification();
        }

        private bool IsSuccessful()
        {
            var manager = (DownloadManager)_context.GetSystemService(Context.DownloadService);
            var query = new DownloadManager.Query();
            query.SetFilterById(_downloadId);
            ICursor cursor = manager.InvokeQuery(query);
            if (cursor.MoveToFirst() && cursor.Count > 0)
            {
                var status = (DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus));

                return status == DownloadStatus.Successful;
            }

            cursor.Close();
            return false;
        }

        private void ConfigAndShowNotification()
        {
            var contentIntent = new Intent(_context, typeof(SplashActivity));
            contentIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            contentIntent.PutExtra("UrgentUpdateDownloadedNotification", true);

            int requestCode = 60; // this is an arbitrary number.
            //requestCode is used to retrieve the same pending intent instance later on (for cancelling, etc).
            //Here we don't actually need it, so this number should not be important.
            //visit: https://stackoverflow.com/a/21526963/5941852

            var contentPendingIntent = PendingIntent.GetActivity(_context, requestCode, contentIntent, PendingIntentFlags.UpdateCurrent);

            // Instantiate the builder and set notification elements:
            NotificationCompat.Builder builder =
                new NotificationCompat.Builder(_context, AppUpdateHelperAndroid.DownloadsNotificationChannelId)
                    .SetContentText(Locale.Resources.ResaNewVersionDownloadedmessage)
                    .SetSmallIcon(Resource.Drawable.resa_icon_notification)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources.System, Resource.Drawable.icon))
                    .SetShowWhen(false)
                    .SetAutoCancel(true)
                    .SetContentIntent(contentPendingIntent);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager = _context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Notify(AppUpdateHelperAndroid.DownloadsNotificationId, notification);
        }

        #region Private Fields

        private long _downloadId;
        private Context _context;

        #endregion
    }
}