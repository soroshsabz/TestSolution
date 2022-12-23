using BSN.Resa.DoctorApp.Commons.DeviceManipulators;
using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Commons.Utilities;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;
using System;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.AppStartConsumers
{
	public class AppStartConsumer : IAppStartConsumer
	{
		public AppStartConsumer(
			IDoctorRepository doctorRepository, 
			IUnitOfWork unitOfWork,
			ICallBlockAndIdentification callBlockAndIdentification)
		{
			_doctorRepository = doctorRepository;
			_unitOfWork = unitOfWork;
			_callBlockAndIdentification = callBlockAndIdentification;
		}

		public void OnStart()
		{
			Doctor doctor = _doctorRepository.Get();

			_callBlockAndIdentification.RefreshBlockedPhoneNumbers();

			try
			{
				doctor.SynchronizeContactsAsync().WaitWithUnwrappedExceptions();

				_doctorRepository.Update();
				_unitOfWork.Commit();
			}
			catch (Exception exception)
				when (exception is UserMustBeLoggedInException || exception is ServiceCommunicationException)
			{
				// ignored
			}
		}

		private readonly IDoctorRepository _doctorRepository;

		private readonly IUnitOfWork _unitOfWork;

		private readonly ICallBlockAndIdentification _callBlockAndIdentification;
	}
}