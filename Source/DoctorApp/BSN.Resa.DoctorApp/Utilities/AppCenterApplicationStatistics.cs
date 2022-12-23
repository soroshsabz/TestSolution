using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using BSN.Resa.DoctorApp.Services;

namespace BSN.Resa.DoctorApp.Utilities
{
    public class AppCenterApplicationStatistics : IApplicationStatistics
    {
        public void SendEvent(string eventName)
        {
            Analytics.TrackEvent(eventName);
        }

        public void SendEvent(string eventName, Dictionary<string, string> properties)
        {
            Analytics.TrackEvent(eventName, properties);
        }
    }
}
