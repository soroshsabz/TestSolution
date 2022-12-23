using BSN.Resa.DoctorApp.Services;
using BSN.Resa.Locale;
using Foundation;
using QuickLook;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;

namespace BSN.Resa.DoctorApp.iOS.Services
{
    public class PhotoViewer : IPhotoViewer
    {
        public Task ShowPhoto(ReadOnlyFile photoFile)
        {
            QLPreviewController previewController = new QLPreviewController
            {
                DataSource = new QuickLookDataSource(photoFile.FullPath)
            };

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(previewController, true, null);

            return Task.CompletedTask;
        }
    }

    public class QuickLookDataSource : QLPreviewControllerDataSource
    {
        private readonly string _path;

        public QuickLookDataSource(string path)
        {
            _path = path;
        }

        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
        {
            return new QuickLookPreviewItem(_path);
        }

        public override nint PreviewItemCount(QLPreviewController controller)
        {
            return 1;
        }
    }

    public class QuickLookPreviewItem : QLPreviewItem
    {
        private readonly string _path;

        public QuickLookPreviewItem(string path)
        {
            _path = path;
        }

        public override string ItemTitle => Resources.ResaDoctorApp;

        public override NSUrl ItemUrl => NSUrl.FromFilename(_path);
    }
}