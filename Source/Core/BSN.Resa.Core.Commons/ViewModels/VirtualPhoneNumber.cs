using System;

namespace BSN.Resa.Core.Commons.ViewModels
{
    public class VirtualPhoneNumber
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsAvailableForPublicUse { get; set; }

        public DateTime CreatedAt { get; set; }

        public VirtualPhoneNumberAssignment OperationalAssignment { get; set; }

        public string OwnerId { get; set; }

        public string OwnerFullName { get; set; }
    }
}
