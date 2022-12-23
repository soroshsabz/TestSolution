using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Telephony;
using BSN.Resa.DoctorApp.Droid.EventConsumers.CallStateChangedConsumers;
using BSN.Resa.DoctorApp.Droid.Services;
using NLog;
using CallState = Android.Telephony.CallState;

namespace BSN.Resa.DoctorApp.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "android.intent.action.PHONE_STATE" })]
    public class IncomingCallListener : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var resaService = new ResaService();

            if (resaService.CanStart())
            {
                resaService.Start();
            }
        }

        public class MyPhoneStateListener : PhoneStateListener
        {
            public enum CallEvent
            {
                Idle,
                Ringing,
                RingingOrMissed,
                Answer,
                Speaking,
                HangUp
            }

            public MyPhoneStateListener(Context context)
            {
                if (Logger == null)
                    Logger = LogManager.GetCurrentClassLogger();
                _lastCallState = CallState.Idle;
                _callEvent = CallEvent.Idle;
            }

            public override void OnCallStateChanged([GeneratedEnum] CallState state, string incomingNumber)
            {
                base.OnCallStateChanged(state, incomingNumber);

                switch (state)
                {
                    case CallState.Ringing:
                        {
                            HandleRingingCase(incomingNumber);

                            break;
                        }
                    case CallState.Offhook:
                        {
                            HandleOffHookCase();

                            break;
                        }
                    case CallState.Idle:
                        {
                            HandleIdleCase();

                            break;
                        }
                }

                _lastCallState = state;
            }

            private void HandleIdleCase()
            {
                Logger.Debug("Idle " + _callEvent);

                if (_callEvent == CallEvent.Answer)
                {
                    Logger.Debug("Answered");
                    _callEvent = CallEvent.HangUp;

                    ResaService.Current
                        .PatientCallHandler.OnCallStateChanged(this, new CallStateChangedEventArges(
                            EventConsumers.CallStateChangedConsumers.CallState.Ended, _phoneNumber));
                }
                else if (_lastCallState == CallState.Ringing)
                {
                    _callEvent = CallEvent.RingingOrMissed;
                }
                else
                    UnexpectedStateSaw();

                CheckNewCallbackRequests();
            }

            private void CheckNewCallbackRequests()
            {
                if (_callEvent != CallEvent.Idle)
                {
                    ResaService.Current.CallbackRequestsChecker.OnCallStateChanged(this,
                        new CallStateChangedEventArges(EventConsumers.CallStateChangedConsumers.CallState.Ended,
                            _phoneNumber));
                }
            }

            private void HandleOffHookCase()
            {
                Logger.Debug("Offhook " + _lastCallState);
                if (_lastCallState == CallState.Ringing)
                    _callEvent = CallEvent.Answer;
                else
                    UnexpectedStateSaw();
            }

            private void HandleRingingCase(string incomingNumber)
            {
                _phoneNumber = incomingNumber;
                ResaService.Current.PatientCallHandler.OnCallStateChanged(
                    this, new CallStateChangedEventArges(
                        EventConsumers.CallStateChangedConsumers.CallState.Started, _phoneNumber));

                Logger.Debug("Ringing");
                _callEvent = CallEvent.Ringing;
            }

            private void UnexpectedStateSaw()
            {
                Logger.Error("Unexpected state: "
                             + nameof(_lastCallState) + ": " + _lastCallState + " "
                             + nameof(_callEvent) + ": " + _callEvent);
            }

            private CallState _lastCallState;

            private CallEvent _callEvent;

            private string _phoneNumber;
        }

        public static ILogger Logger;
    }
}