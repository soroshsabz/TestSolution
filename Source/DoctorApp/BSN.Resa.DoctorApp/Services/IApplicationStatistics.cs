using System.Collections.Generic;

namespace BSN.Resa.DoctorApp.Services
{
    public interface IApplicationStatistics
    {
        void SendEvent(string eventName);

        void SendEvent(string eventName, Dictionary<string, string> properties);
    }
}