using System;

namespace BSN.Resa.DoctorApp.iOS.EventConsumers.UrlCallConsumers
{
	public class InvalidArgumentException : Exception
	{
		public InvalidArgumentException(string message = null) :
			base(message)
		{ }
	}
}