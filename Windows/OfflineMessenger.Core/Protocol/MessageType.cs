using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Core.Protocol;

public enum MessageType
{
    HandshakeInit = 1,
    HandshakeReply = 2,
    Chat = 3
}