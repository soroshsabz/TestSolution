using System;
using System.Collections.Generic;

namespace BSN.Resa.Commons.ViewModels.CentralAuthenticationService.TokenBasedAuthentication
{
    public class AccessTokenForm
    {
        public string AccountId { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string Title { get; set; }

        public List<int> AuthorizationScopeIds { get; set; }
    }
}
