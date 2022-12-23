namespace BSN.Resa.DoctorApp.Commons.DeviceManipulators
{
	public interface IMobileContactManager
	{
		void AddContact(string firstName, string lastName, string[] phoneNumbers, string imageFileName);

		bool Contains(string phoneNumber);
	}
}
