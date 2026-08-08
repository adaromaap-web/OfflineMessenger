using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Core.Discovery;

public class DiscoveryService
{
    public event Action<PeerDevice>? DeviceFound;



    public void DeviceDetected(
        string deviceId
    )
    {
        var device =
            new PeerDevice(
                deviceId
            );


        DeviceFound?.Invoke(
            device
        );
    }
}
