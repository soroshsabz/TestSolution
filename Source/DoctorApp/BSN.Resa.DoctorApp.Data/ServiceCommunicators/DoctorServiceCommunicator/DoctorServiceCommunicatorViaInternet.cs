using BSN.Resa.DoctorApp.Commons;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.ServiceCommunicators;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using Plugin.Connectivity.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Data.ServiceCommunicators.DoctorServiceCommunicator
{
    public class DoctorServiceCommunicatorViaInternet : AbstractDoctorServiceCommunicatorHandler
    {
        #region Constructor

        public DoctorServiceCommunicatorViaInternet(
            IConnectivity connectivity,
            IConfig config,
            ConnectionStatusManager connectionStatusManager,
            INativeHttpMessageHandlerProvider nativeHttpMessageHandlerProvider
            ) : base(connectivity, config, connectionStatusManager, nativeHttpMessageHandlerProvider)
        { }

        #endregion

        public override event OnSendDoctorStateResult OnSendDoctorStateResult;

        public override async Task SendDoctorStateAsync(Commons.DoctorState state)
        {
            try
            {
                await SendRequestAsync($"Doctors?State={(int)state}", HttpMethod.Put).ConfigureAwait(false);

                OnSendDoctorStateResult?.Invoke(state: state, isSuccessful: true);
            }
            catch (NetworkConnectionException)
            {
                if (NextBaseCommunicator != null)
                {
                    NextBaseCommunicator.OnSendDoctorStateResult += OnSendDoctorStateResult;

                    await NextBaseCommunicator.SendDoctorStateAsync(state);
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
                await SendRequestAsync("Doctors/BlockedConsumers", HttpMethod.Post,
                    phoneNumbers.ToNationalPhoneNumberFormat()).ConfigureAwait(false);
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
        }
    }
}