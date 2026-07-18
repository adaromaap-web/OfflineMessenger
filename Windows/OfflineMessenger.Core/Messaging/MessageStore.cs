using System;
using System.Collections.Generic;
using OfflineMessenger.Core.Protocol;

namespace OfflineMessenger.Core.Messaging;

public class MessageStore
{
    private readonly Dictionary<Guid, MessagePacket> _messages = new();


    public void Add(MessagePacket message)
    {
        _messages[message.MessageId] = message;
    }


    public bool TryGet(
        Guid messageId,
        out MessagePacket message)
    {
        return _messages.TryGetValue(
            messageId,
            out message
        );
    }


    public void UpdateStatus(
        Guid messageId,
        MessageStatus status)
    {
        if (_messages.TryGetValue(
                messageId,
                out var message))
        {
            message.Status = status;
        }
    }


    public IReadOnlyCollection<MessagePacket> GetAll()
    {
        return _messages.Values;
    }
}