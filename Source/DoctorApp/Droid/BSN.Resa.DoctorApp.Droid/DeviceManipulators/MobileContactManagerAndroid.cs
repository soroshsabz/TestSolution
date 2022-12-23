using Android.Content;
using Android.Database;
using Android.Provider;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Droid.Helpers;
using BSN.Resa.DoctorApp.Services;
using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Droid.DeviceManipulators
{
    public class MobileContactManagerAndroid : IMobileContactManager
    {
        public MobileContactManagerAndroid(ICrashReporter crashReporter)
        {
            _crashReporter = crashReporter;
        }

        // Visit https://forums.xamarin.com/discussion/3950/adding-a-contact-xamarin-example
        public void AddContact(string firstName, string lastName, string[] phoneNumbers, string imagePath = null)
        {
            var contentProviderOperations = new List<ContentProviderOperation>();

            ContentProviderOperation.Builder builder =
                ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
            builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
            builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
            contentProviderOperations.Add(builder.Build());

            // Name
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                              ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, lastName);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, firstName);
            contentProviderOperations.Add(builder.Build());

            // Number
            foreach (string phoneNumber in phoneNumbers)
            {
                builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                                  ContactsContract.CommonDataKinds.Phone.ContentItemType);
                builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, phoneNumber);
                builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
                                  ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
                builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Work");
                contentProviderOperations.Add(builder.Build());
            }

            // Image
            if (imagePath != null)
            {
                builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                builder.WithValue(ContactsContract.Data.InterfaceConsts.IsSuperPrimary, 1);
                builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Photo.ContentItemType);
                builder.WithValue(ContactsContract.CommonDataKinds.Photo.PhotoColumnId, ResourceHelper.ReadAllBytes(imagePath, "drawable"));
                contentProviderOperations.Add(builder.Build());
            }

            //Add the new contact
            try
            {
                Context.ContentResolver.ApplyBatch(ContactsContract.Authority, contentProviderOperations);
            }
            catch (Exception exception)
            {
                _crashReporter.SendException(exception);
            }
        }

        public bool Contains(string phoneNumber)
        {
            Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath(ContactsContract.PhoneLookup.ContentFilterUri, Android.Net.Uri.Encode(phoneNumber));
            ICursor cursor = Context.ContentResolver.Query(uri, new[] { ContactsContract.PhoneLookup.InterfaceConsts.DisplayName }, null, null, null);
            return (cursor != null && cursor.MoveToFirst());
        }

        private readonly ICrashReporter _crashReporter;
        private Context Context => Android.App.Application.Context;
    }
}