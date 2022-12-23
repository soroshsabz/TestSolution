using System;
using System.Collections.Generic;

namespace BSN.Resa.Commons.ViewModels.CentralAuthenticationService.TokenBasedAuthentication
{
    public class AccessToken
    {
        public int Id { get; set; }
        
        public string Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }
        
        public virtual ICollection<AuthorizationScope> AuthorizationScopes { get; set; }

        public virtual ICollection<ConnectionFilter> ConnectionFilters { get; set; }

        public string Title { get; set; }

        public bool IsValid { get; set; }

        public AuthorizedTokenConsumer Owner { get; set; }
    }
}