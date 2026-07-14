using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Core.Protocol;

public class AckPacket
{
    public Guid SessionId { get; set; }

    public Guid MessageId { get; set; }

    public bool Received { get; set; }
}