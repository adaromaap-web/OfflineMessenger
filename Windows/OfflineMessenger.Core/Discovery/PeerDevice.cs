using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Core.Discovery;

public class PeerDevice
{
    public string Id { get; }


    public PeerDevice(
        string id
    )
    {
        Id = id;
    }
}