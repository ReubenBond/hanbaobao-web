using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApp
{
    internal interface IUserAgentGrain : IGrainWithStringKey
    {
    }

    internal class UserAgentGrain : Grain, IUserAgentGrain
    {
        // Anti-abuse, identity, notifications
    }
}
