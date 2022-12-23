using BSN.Resa.DoctorApp.Commons.Exceptions;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;
using BSN.Resa.DoctorApp.Services;
using System;

namespace BSN.Resa.DoctorApp.EventConsumers.InternetAccessedConsumers
{
	public class PhoneNumbersSynchronizer : IInternetAccessedConsumer
	{
		public PhoneNumbersSynchronizer(
			IDoctorRepository doctorRepository,
			IUnitOfWork unitOfWork,
            ICrashReporter crashReporter)
		{
			_doctorRepository = doctorRepository;
			_unitOfWork = unitOfWork;
            _crashReporter = crashReporter;
        }

		public async void OnInternetAccessed()
		{
			Doctor doctor = _doctorRepository.Get();

		    try
		    {
		        await doctor.SynchronizeContactsAsync().ConfigureAwait(false);

		        _doctorRepository.Update();
		        _unitOfWork.Commit();
		    }
		    catch (Exception exception)
		        when (exception is ServiceCommunicationException || exception is UserMustBeLoggedInException)
		    {
		        // ignored
		    }
		    catch(Exception exception)
		    {
                _crashReporter.SendException(exception);
            }
		}

        #region Private Fields

        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICrashReporter _crashReporter;

        #endregion
    }
}