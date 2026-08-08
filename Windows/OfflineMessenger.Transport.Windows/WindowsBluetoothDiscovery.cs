using InTheHand.Net;
using OfflineMessenger.Core.Discovery;

namespace OfflineMessenger.Transport.Windows;

public class WindowsBluetoothDiscovery
{
    private readonly DiscoveryService _discoveryService;


    public WindowsBluetoothDiscovery(
        DiscoveryService discoveryService)
    {
        _discoveryService = discoveryService;
    }



    public async Task ScanAsync()
    {
        await Task.Run(() =>
        {
            


          


            
        });
    }
}