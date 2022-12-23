using System.Threading.Tasks;

namespace BSN.Resa.DoctorApp.Commons.DeviceManipulators
{
    public interface ICallBlockAndIdentification
    {
		bool IsBlockingEnabled { get; }

		bool IsIdentificationEnabled { get; }

		void RefreshBlockedPhoneNumbers();
	}
}
