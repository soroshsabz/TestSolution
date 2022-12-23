namespace BSN.Resa.DoctorApp.Utilities
{
    public interface IApplicationManipulator
	{
		bool CanCloseApplicationGracefully { get; }

		void CloseApplicationGracefully();

	    void CloseApplication();
    }
}
