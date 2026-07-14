using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Transport.Abstractions;

public interface ITransport
{
    Task ConnectAsync(string deviceId);
    Task SendAsync(byte[] data);

    event Action<byte[]> DataReceived;
}