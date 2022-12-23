using System;
using BSN.Resa.DoctorApp.Utilities;

namespace BSN.Resa.DoctorApp.iOS.LocalMarkets.Utilities
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