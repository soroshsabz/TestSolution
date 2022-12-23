using System.Threading.Tasks;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using BSN.Resa.DoctorApp.Data.Repository;
using BSN.Resa.DoctorApp.Domain.Models;

namespace BSN.Resa.DoctorApp.Droid.EventConsumers.PatientAuthenticationInquirerAnsweredConsumers
{
	public class PatientAuthenticationInquirerAnsweredConsumer : IPatientAuthenticationInquirerAnsweredConsumer
	{
		public PatientAuthenticationInquirerAnsweredConsumer(
			IDoctorRepository doctorRepository,
			IUnitOfWork unitOfWork)
		{
			_doctorRepository = doctorRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task OnAnswered(object sender, PatientAuthenticationInquirerAnsweredEventArgs e)
		{
			if (e.Result == PatientAuthenticationInquirerResult.Unknown)
				return;

			Doctor doctor = _doctorRepository.Get();

			if (e.Result == PatientAuthenticationInquirerResult.Yes)
				await doctor.AddOrUpdateContactAsync(Contact.Blocked(e.PhoneNumber)).ConfigureAwait(false);
			else if (e.Result == PatientAuthenticationInquirerResult.No)
				await doctor.AddOrUpdateContactAsync(Contact.Allowed(e.PhoneNumber)).ConfigureAwait(false);

			_doctorRepository.Update();
			_unitOfWork.Commit();
		}

		private readonly IDoctorRepository _doctorRepository;

		private readonly IUnitOfWork _unitOfWork;
	}
}
