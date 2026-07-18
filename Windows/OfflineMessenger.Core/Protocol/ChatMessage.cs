using System;

namespace OfflineMessenger.Core.Protocol;

public class ChatMessage
{
    public Guid MessageId { get; set; }

    public Guid SessionId { get; set; }

    public string Text { get; set; }

    public long Timestamp { get; set; }

    public MessageStatus Status { get; set; }
}