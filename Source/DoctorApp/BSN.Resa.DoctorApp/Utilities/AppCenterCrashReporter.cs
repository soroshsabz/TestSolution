using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using BSN.Resa.DoctorApp.Services;

namespace BSN.Resa.DoctorApp.Utilities
{
    public class AppCenterCrashReporter : ICrashReporter
    {
        public void SendException(Exception exception)
        {
            Crashes.TrackError(exception);
        }

        public void SendException(Exception exception, Dictionary<string, string> properties)
        {
            Crashes.TrackError(exception, properties);
        }
    }
}