using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class CommunicationPermitViewModel
    {
        public string BookingId { get; set; }

        public TimeSpan MaximumCommunicationDuration { get; set; }
    }
}
