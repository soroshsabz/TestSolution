using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.iOS.Commons;
using BSN.Resa.DoctorApp.iOS.Commons.Utilities;
using BSN.Resa.Locale;
using Foundation;
using System;
using System.Linq;
using UIKit;

namespace BSN.Resa.DoctorApp.iOS.ShareExtension
{
    // https://stackoverflow.com/questions/34260049/how-to-create-an-ios-share-extension-for-contacts
    public partial class ShareViewController : UIViewController
	{
		#region Constants

		private const string ContactType = "public.vcard";

		#endregion

		#region Constructors

		public ShareViewController(IntPtr handle) : base(handle)
		{}

		#endregion

		#region Methods

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.Hidden = true;

            if (!IsValidItems())
                return;

            ExtensionContext.InputItems[0].Attachments[0].LoadItem(ContactType, null, (item, error) =>
			{
				if (error == null)
				{
                    string text = (item as NSData)?.ToString(NSStringEncoding.UTF8).ToString();

					string[] phoneNumbers = text.FindPhoneNumbers().ToArray();

					BeginInvokeOnMainThread(() =>
					{
						SendPhoneNumbersToContainerApp(phoneNumbers);
						NavigationController.PopViewController(false);
					});
				}
			});
		}

		private bool IsValidItems()
		{
			NSItemProvider[] itempProviders = ExtensionContext.InputItems.Length > 0 ?
									ExtensionContext.InputItems[0].Attachments :
									null;
			
			if (itempProviders == null ||
				itempProviders.Length == 0 ||
				!itempProviders[0].HasItemConformingTo(ContactType))
			{
				var alertController = new UIAlertController
				{
					Title = Resources.SelectedContentIsNotSharableWithResaMessage
                };
				alertController.AddAction(UIAlertAction.Create(Resources.Ok, UIAlertActionStyle.Cancel, action =>
				{
					NavigationController.PopViewController(false);
				}));
				PresentViewController(alertController, true, null);
				return false;
			}

			return true;
		}

		private void SendPhoneNumbersToContainerApp(string[] phoneNumbers)
		{
			var urlComponents = new NSUrlComponents
			{
				Scheme = ConfigiOS.Instance.UrlScheme,
				Host = ConfigiOS.Instance.ShareContactUrlIdentifier,
				Query = $"phoneNumbers=\"[{phoneNumbers.ToString(",")}]\""
			};

			if (UIApplication.SharedApplication.CanOpenUrl(urlComponents.Url))
			{
                UIApplication.SharedApplication.OpenUrl(urlComponents.Url);
            }
		}

		#endregion
	}
}