using System;

namespace BSN.Resa.Commons.ViewModels.CentralAuthenticationService.TokenBasedAuthentication
{
    public class ConnectionFilter
    {
        public int Id { get; set; }

        public string ConnectionIdentifier { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsValid { get; set; }

        public bool IsDomainName { get; set; }

        public int OwnerId { get; set; }

        public string OwnerAccountId { get; set; }
    }
}
