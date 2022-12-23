// ITNOA

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BSN.Resa.DoctorApp.Data.Infrastructure
{
	public interface INativeHttpMessageHandlerProvider
	{
		HttpMessageHandler Get();
	}
}
