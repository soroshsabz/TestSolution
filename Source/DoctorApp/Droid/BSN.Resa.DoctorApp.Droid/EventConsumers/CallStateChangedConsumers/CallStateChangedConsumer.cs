using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Droid.Utilities;
using BSN.Resa.DoctorApp.Services;
using PhoneNumbers;
using System.Linq;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.CallStateChangedConsumers
{
    public class CallStateChangedConsumer : ICallStateChangedConsumer
	{
        #region Constructor

        public CallStateChangedConsumer(
            IDoctorRepository doctorRepository,
            IIncomingCallHelper incomingCallHelper,
            IMobileContactManager mobileContactManager,
            IUnitOfWork unitOfWork,
            IPermissionsManager permissionsManager)
        {
            _doctorRepository = doctorRepository;
            _incomingCallHelper = incomingCallHelper;
            _mobileContactManager = mobileContactManager;
            _unitOfWork = unitOfWork;
            _permissionsManager = permissionsManager;
        }

        #endregion

        public async void OnCallStateChanged(object sender, CallStateChangedEventArges e)
		{
            bool areRequiredPermissionsGranted = await _permissionsManager.AreAllPermissionsGrantedAsync();

            if (!areRequiredPermissionsGranted)
            {
                _permissionsManager.RedirectUserToPermissionsPage();

                return;
            }

            if (_mobileContactManager.Contains(e.PhoneNumber))
				return;

		    Doctor doctor = _doctorRepository.Get();

		    if (!doctor.IsLoggedIn)
				return;

			string phoneNumber;
			try
			{
				phoneNumber = e.PhoneNumber.ToE164PhoneNumberFormat();
			}
			catch(NumberParseException)
			{
				return;
			}

			Contact contact = doctor.Contacts.FirstOrDefault(c => c.PhoneNumber == phoneNumber);

			if(e.State == CallState.Started && contact?.IsBlocked == true)
			{
				_incomingCallHelper.MuteRinging();
				_incomingCallHelper.DisconnectCall();
                try
				{
					await doctor.AnnounceContactIfNotAnnouncedYetAsync(phoneNumber).ConfigureAwait(false);
					contact.IncreaseBlockedCount();

					_doctorRepository.Update();
					_unitOfWork.Commit();
				}
				catch(ServiceCommunicationException)
				{
					// ignored
				}
			}
			else if(e.State == CallState.Ended && e.PhoneNumber.IsValidPhoneNumber() && contact == null)
			{
				_incomingCallHelper.ShowPatientAuthenticationInquirerDialog(phoneNumber);
			}
		}

        #region Private Fields

        private readonly IIncomingCallHelper _incomingCallHelper;
        private readonly IMobileContactManager _mobileContactManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionsManager _permissionsManager;
        private readonly IDoctorRepository _doctorRepository;

        #endregion
    }
}