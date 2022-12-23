using System;
using System.Net.Http;

namespace BSN.Resa.Commons
{
	public class ConnectionException : Exception
	{
		public HttpResponseMessage Response { get; set; }
		public ConnectionException(HttpResponseMessage response = null, string message = "ConnectionException"): base(message)
		{
			Response = response;
		}
	}
}