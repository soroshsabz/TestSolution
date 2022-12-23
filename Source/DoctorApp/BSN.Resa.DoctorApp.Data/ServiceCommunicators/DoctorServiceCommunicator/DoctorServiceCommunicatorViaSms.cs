using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using Plugin.Connectivity.Abstractions;
using Plugin.Messaging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Data.ServiceCommunicators.DoctorServiceCommunicator
{
    public class DoctorServiceCommunicatorViaSms : AbstractDoctorServiceCommunicatorHandler
    {
        #region Constructor

        public DoctorServiceCommunicatorViaSms(
            IConnectivity connectivity,
            IConfig config,
            ConnectionStatusManager connectionStatusManager,
            INativeHttpMessageHandlerProvider nativeHttpMessageHandlerProvider,
            IGsmConnection gsmConnection,
            ISmsTask smsTask) : base(connectivity, config, connectionStatusManager, nativeHttpMessageHandlerProvider)
        {
            _gsmConnection = gsmConnection;
            _smsTask = smsTask;
        }

        #endregion

        public override event OnSendDoctorStateResult OnSendDoctorStateResult;

        public override async Task SendDoctorStateAsync(Commons.DoctorState state)
        {
            _currentDoctorState = state;

            try
            {
                CheckPreconditions();

                string smsContent = $"Doctors@@{CommunicationToken.Vsin}@@UpdateState@@state={(int)state}";
                string recipient = Config.ResaSmsReceiverPhoneNumber;

                _smsTask.OnSmsDeliveryResult += OnSmsDeliveryResult;

                await Task.Run(() => _smsTask.SendSmsInBackground(recipient, Base64Encode(smsContent)));
            }
            catch (NetworkConnectionException)
            {
                _smsTask.OnSmsDeliveryResult -= OnSmsDeliveryResult;

                if (HasNextCommunicator())
                {
                    await DelegateSendDoctorStateToNextCommunicatorAsync(state);
                }
                else
                {
                    OnSendDoctorStateResult?.Invoke(state: state, isSuccessful: false);

                    throw;
                }
            }
        }

        public override async Task SendBlockedPhoneNumbersAsync(string[] phoneNumbers)
        {
            try
            {
                if (phoneNumbers.Length <= 0)
                    return;

                CheckPreconditions();

                foreach (var phoneNumber in phoneNumbers)
                {
                    string smsContent =
                        $"Doctors@@{CommunicationToken.Vsin}@@AssociateBlockedConsumers@@consumerPhoneNumbers={phoneNumber}";
                    string recipient = Config.ResaSmsReceiverPhoneNumber;
                    await Task.Run(() => _smsTask.SendSmsInBackground(recipient, Base64Encode(smsContent)))
                        .ConfigureAwait(false);

                }
            }
            catch (NetworkConnectionException)
            {
                if (NextBaseCommunicator != null)
                {
                    await NextBaseCommunicator.SendBlockedPhoneNumbersAsync(phoneNumbers);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception exception) when (exception.GetType().ToString().ToLowerInvariant() == "java.lang.securityexception")
            {
                //Sending an SMS requires SMS Permission to be granted by user otherwise the exception "Java.Lang.SecurityException"
                //will be thrown. But the problem is that's a native Android exception not a managed C# exception. And in this
                //catch block we convert it to a managed one. Also since this security exception is anticipated and expected and
                //not a fatal one, then this method caller would behave appropriately.

                if (NextBaseCommunicator != null)
                {
                    await NextBaseCommunicator.SendBlockedPhoneNumbersAsync(phoneNumbers);
                }
                else
                {
                    var convertedException = new ServiceCommunicationException(message: "SMS permission is not granted",
                        innerException: exception);

                    throw convertedException;
                }
            }
        }

        #region Private Methods

        private async void OnSmsDeliveryResult(bool result)
        {
            _smsTask.OnSmsDeliveryResult -= OnSmsDeliveryResult;

            if (result == true)
            {
                OnSendDoctorStateResult?.Invoke(_currentDoctorState, isSuccessful: true);
            }
            else
            {
                await OnSmsDeliveryFailed(_currentDoctorState);
            }
        }

        private async Task OnSmsDeliveryFailed(Commons.DoctorState state)
        {
            if (HasNextCommunicator())
            {
                await DelegateSendDoctorStateToNextCommunicatorAsync(state);
            }
            else
            {
                OnSendDoctorStateResult?.Invoke(state: state, isSuccessful: false);
            }
        }

        private async Task DelegateSendDoctorStateToNextCommunicatorAsync(Commons.DoctorState state)
        {
            NextBaseCommunicator.OnSendDoctorStateResult += OnSendDoctorStateResult;

            await NextBaseCommunicator.SendDoctorStateAsync(state);
        }

        private bool HasNextCommunicator()
        {
            return NextBaseCommunicator != null;
        }

        private void CheckPreconditions()
        {
            CheckGsmConnection();
            CheckCanSendSms();
        }

        private void CheckGsmConnection()
        {
            if (!_gsmConnection.IsConnected)
                throw new NoGsmConnectionException();
        }

        private void CheckCanSendSms()
        {
            if (!_smsTask.CanSendSmsInBackground)
                throw new SmsSendingException();
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        #endregion

        #region Private Fields

        private readonly ISmsTask _smsTask;
        private readonly IGsmConnection _gsmConnection;
        private Commons.DoctorState _currentDoctorState;

        #endregion
    }
}