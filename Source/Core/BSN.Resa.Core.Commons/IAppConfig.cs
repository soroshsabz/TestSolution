using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Core.Commons
{
    public interface IAppConfig
    {
        string Get(string key);
    }
}
