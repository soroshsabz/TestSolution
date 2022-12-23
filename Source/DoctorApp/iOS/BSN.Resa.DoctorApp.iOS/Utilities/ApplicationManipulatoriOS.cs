using BSN.Resa.DoctorApp.Utilities;
using System;

namespace BSN.Resa.DoctorApp.iOS.Utilities
{
    public class ApplicationManipulatoriOS: IApplicationManipulator
	{
		public bool CanCloseApplicationGracefully => false;

		public void CloseApplicationGracefully()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Notice this method sometimes seems as application 
		/// crash in iOS, So don't use it as much as you can.
		/// </summary>
		public void CloseApplication()
		{
			System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
			//Thread.CurrentThread.Abort();
		}
	}
}