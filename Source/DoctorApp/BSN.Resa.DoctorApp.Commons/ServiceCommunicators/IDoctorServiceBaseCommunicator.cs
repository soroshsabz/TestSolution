using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Commons.ServiceCommunicators
{
    public delegate void OnSendDoctorStateResult(DoctorState state, bool isSuccessful);

    /// <summary>
    /// This is an interface for which each method will have a different mechanism of implementation,
    /// for example one with via SMS, the other via Internet.
    /// </summary>
    public interface IDoctorServiceBaseCommunicator
    {
        event OnSendDoctorStateResult OnSendDoctorStateResult;

        Task SendBlockedPhoneNumbersAsync(string[] phoneNumbers);

        Task SendDoctorStateAsync(DoctorState doctorState);

        DoctorServiceCommunicationToken CommunicationToken { get; set; }
    }
}