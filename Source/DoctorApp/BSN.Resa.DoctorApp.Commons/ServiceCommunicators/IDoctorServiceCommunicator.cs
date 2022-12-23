using BSN.Resa.DoctorApp.Commons.MedicalTestViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
    /// <summary>
    /// This is an interface that all methods are implemented via a specific mechanism i.e internet.
    /// </summary>
    public interface IDoctorServiceCommunicator : IDoctorServiceBaseCommunicator
    {
        IDoctorServiceBaseCommunicator SetNextCommunicator(IDoctorServiceBaseCommunicator nextBaseCommunicator);

        Task<DoctorState> GetDoctorStateAsync(CancellationToken cancellationToken = default);

        Task<string[]> GetBlockedPhoneNumbersAsync();

        Task<string[]> GetAllowedPhoneNumbersAsync();

        Task SendAllowedPhoneNumbersAsync(string[] phoneNumbers);

        Task RemoveBlockedPhoneNumbersAsync(string[] phoneNumbers);

        Task RemoveAllowedPhoneNumbersAsync(string[] phoneNumbers);

        Task<string[]> GetResaPhoneNumbersAsync();

        Task<DoctorPreviewViewModel> GetDoctorPreviewAsync(CancellationToken cancellationToken = default);

        Task<ICollection<CallbackRequestPreview>> GetAllCallbackRequestsAsync(int offset = 0, int limit = int.MaxValue, CancellationToken cancellationToken = default);

        Task<ICollection<CallbackRequestPreview>> GetActiveCallbackRequestsAsync(int offset = 0, int limit = int.MaxValue, CancellationToken cancellationToken = default);

        Task BookCallbackRequestAsync(string callbackId, CancellationToken cancellationToken = default);

        Task<string> GetCallbackRequestPhoneNumber(CancellationToken cancellationToken = default);

        Task<ICollection<MedicalTestViewModel>> GetAllActiveMedicalTests(int offset = 0, int limit = int.MaxValue, CancellationToken cancellationToken = default);

        Task SubmitMedicalTestTextReply(int medicalTestId, string reply, CancellationToken cancellationToken = default);

        Task SubmitMedicalTestVoiceReply(int medicalTestId, FileStream voiceFileStream, string voiceFileNameWithExtension, CancellationToken cancellationToken = default);
    }
}