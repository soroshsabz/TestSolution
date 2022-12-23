using System;
using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Services
{
    public interface ICrashReporter
    {
        void SendException(Exception exception);

        void SendException(Exception exception, Dictionary<string, string> properties);
    }
}