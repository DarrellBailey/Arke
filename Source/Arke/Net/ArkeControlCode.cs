using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Net
{
    internal enum ArkeControlCode : byte
    {
        Message = 0,

        SubscribeTopic = 1,

        UnsubscribeTopic = 2,

        PublishTopic = 3,

        Request = 4,

        Response = 5
    }
}
