using System;

namespace BSN.Resa.Commons.ViewModels.CentralAuthenticationService.TokenBasedAuthentication
{
    public class AuthorizedTokenConsumer
    {
        public int Id { get; set; }

        public string AccountId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SynchronizedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
