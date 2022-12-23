using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Telecom;
using Android.Views;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Droid.EventConsumers.PatientAuthenticationInquirerAnsweredConsumers;
using BSN.Resa.DoctorApp.Droid.Utilities;
using System;

namespace BSN.Resa.DoctorApp.Droid.Services
{
    // ReSharper disable once InconsistentNaming
    public partial class ResaService
    {
		public void DisconnectCall()
        {
            /*
             * From Android version 8 and on using reflection method(else block in below code) for ending calls will result in:
             * java.lang.SecurityException: MODIFY_PHONE_STATE permission required
             * Visit: https://stackoverflow.com/a/50735559/5941852
             * 
             * Note: the new version method is deprecated in Android 9! DAMN IT!
             * 
             * TODO: Please use CallScreeingService API instead
             * https://developer.android.com/reference/android/telecom/CallScreeningService
             */
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                TelecomManager telecomManager =
                    (TelecomManager)Application.Context.GetSystemService(Context.TelecomService);
#if DEBUG
#pragma warning disable CS0618 // Type or member is obsolete
#endif
                telecomManager?.EndCall();
#if DEBUG
#pragma warning restore CS0618 // Type or member is obsolete
#endif
            }
            else
            {
                /*
             * For more information about below codes please see
             * http://stackoverflow.com/questions/17427174/how-to-end-incoming-call-in-monodroid
             */
                IntPtr getITelephonyMethod = JNIEnv.GetMethodID(_telephonyManager.Class.Handle, name: "getITelephony",
                    signature: "()Lcom/android/internal/telephony/ITelephony;");

                IntPtr telephony = JNIEnv.CallObjectMethod(_telephonyManager.Handle, getITelephonyMethod);
                IntPtr iTelephonyClass = JNIEnv.GetObjectClass(telephony);
                IntPtr iTelephonyEndCallMethod = JNIEnv.GetMethodID(iTelephonyClass, name: "endCall", signature: "()Z");

                // TODO: We must have a appropriately answer for the result of this call. (return value)
                JNIEnv.CallBooleanMethod(telephony, iTelephonyEndCallMethod);

                // Release acquire resource in this context
                JNIEnv.DeleteLocalRef(telephony);
                JNIEnv.DeleteLocalRef(iTelephonyClass);
            }

            RecoverRinging();
        }

        public void MuteRinging()
        {
            //Turning on DoNotDisturbMe mode. This is added in API 23
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                _interruptionFilter = _notificationManager.CurrentInterruptionFilter;
                _notificationManager?.SetInterruptionFilter(InterruptionFilter.None);
            }
            else
            {
                _ringerMode = _audioManager.RingerMode;
                _audioManager.RingerMode = RingerMode.Silent;
            }
        }

        private void RecoverRinging()
        {
            //Returning DoNotDisturb mode to its original mode before blocking call. This is added in API 23
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                _notificationManager.SetInterruptionFilter(_interruptionFilter);
            }
            else
            {
                //There seems to be a bug on Lollipop devices(Android 5.0), in that setting ringer mode as silent
                //will put the device in Priority mode (marked by a star on notification bar).
                //Visit: https://stackoverflow.com/questions/32561989/how-to-programmatically-mute-silent-devices-running-lollipop#comment54661467_32624013
                _audioManager.RingerMode = _ringerMode;
            }
        }

        private class IncomingCallHelperAndroid : IIncomingCallHelper
        {
            public void DisconnectCall()
            {
                ResaService.Current.DisconnectCall();
            }

            public void MuteRinging()
            {
                ResaService.Current.MuteRinging();
            }

            public void ShowPatientAuthenticationInquirerDialog(string phoneNumber)
            {
                string message =
                    $"{Locale.Resources.DoctorAppWasTheCallerWithTheNumber} \"{phoneNumber.ToNationalPhoneNumberFormat()}\" {Locale.Resources.AtTheTime} {DateTime.Now:HH:mm} {Locale.Resources.DoctorAppWasOneOfYourPatients}";

                AlertDialog.Builder builder = new AlertDialog.Builder(ResaService.Current._context);
                builder.SetTitle(Locale.Resources.Resa)
                    .SetIcon(Resource.Drawable.resa_alert_dialog_icon)
                    .SetMessage(message)
                    .SetPositiveButton(Locale.Resources.Yes,
                        (s, args) =>
                        {
                            ResaService
                                .Current
                                .PatientAuthenticationInquirerAnsweredConsumer
                                .OnAnswered(
                                    this,
                                    new PatientAuthenticationInquirerAnsweredEventArgs(
                                        phoneNumber,
                                        PatientAuthenticationInquirerResult.Yes)
                                );
                        }).SetNegativeButton(Locale.Resources.No,
                        (s, args) =>
                        {
                            ResaService
                                .Current
                                .PatientAuthenticationInquirerAnsweredConsumer
                                .OnAnswered(
                                    this,
                                    new PatientAuthenticationInquirerAnsweredEventArgs(
                                        phoneNumber,
                                        PatientAuthenticationInquirerResult.No)
                                );
                        })
                    .SetNeutralButton(Locale.Resources.DoctorAppIdontKnowYet, (s, args) =>
                    {
                        ResaService
                            .Current
                            .PatientAuthenticationInquirerAnsweredConsumer
                            .OnAnswered(
                                this,
                                new PatientAuthenticationInquirerAnsweredEventArgs(
                                    phoneNumber,
                                    PatientAuthenticationInquirerResult.Unknown)
                            );
                    }).SetCancelable(false).Create();

                AlertDialog alert = builder.Create();

                //visit: https://stackoverflow.com/a/34061521/5941852
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    alert.Window.SetType(WindowManagerTypes.ApplicationOverlay);
                }
                else
                {
                    alert.Window.SetType(WindowManagerTypes.SystemAlert);
                }

                alert.Show();
            }
        }
    }
}