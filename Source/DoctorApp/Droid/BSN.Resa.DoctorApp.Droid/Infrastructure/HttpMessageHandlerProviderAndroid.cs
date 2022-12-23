using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BSN.Resa.DoctorApp.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Android.Net;

namespace BSN.Resa.DoctorApp.Droid.Infrastructure
{
	internal class HttpMessageHandlerProviderAndroid : INativeHttpMessageHandlerProvider
	{
		public HttpMessageHandler Get()
		{
			return new AndroidClientHandler();
		}
	}
}