using OfflineMessenger.Core.Protocol;
using System;
using System.Text;

namespace OfflineMessenger.Core.Messaging;

public class MessageService
{
    public MessagePacket CreateOutgoingMessage(
        string text,
        byte[] encryptedPayload,
        byte[] nonce,
        byte[] tag,
        Guid sessionId)
    {
        return new MessagePacket
        {
            Type = MessageType.Chat,

            SessionId = sessionId,

            MessageId = Guid.NewGuid(),

            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),

            Status = MessageStatus.Created,

            Direction = MessageDirection.Outgoing,

            Payload = encryptedPayload,

            Nonce = nonce,

            Tag = tag
        };
    }

    public string DecodeIncomingMessage(
    byte[] decryptedPayload)
    {
        return Encoding.UTF8.GetString(decryptedPayload);
    }
}