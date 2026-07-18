using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Core.Protocol;

public class MessagePacket
{
    public MessageType Type { get; set; }
    public Guid SessionId { get; set; }
    public Guid MessageId { get; set; }
    public long Timestamp { get; set; }

    public byte[] Payload { get; set; }

    public MessageStatus Status { get; set; }

    public MessageDirection Direction { get; set; }

    // 🔐 ДОБАВЛЯЕМ
    public byte[] Nonce { get; set; }
    public byte[] Tag { get; set; }
}