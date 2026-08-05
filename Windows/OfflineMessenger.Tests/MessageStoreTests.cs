using OfflineMessenger.Core.Messaging;
using OfflineMessenger.Core.Protocol;
using Xunit;

namespace OfflineMessenger.Tests;

public class MessageStoreTests
{
    // Проверка добавления и получения сообщения

    [Fact]
    public void AddMessage_TryGet_ReturnsMessage()
    {
        var store = new MessageStore();


        var packet = new MessagePacket
        {
            MessageId = Guid.NewGuid(),

            Payload = new byte[]
            {
                1, 2, 3
            },

            Status = MessageStatus.Sent
        };


        store.Add(packet);


        var result =
            store.TryGet(
                packet.MessageId,
                out var restored
            );


        Assert.True(result);


        Assert.Equal(
            packet.MessageId,
            restored.MessageId
        );


        Assert.Equal(
            packet.Payload,
            restored.Payload
        );
    }

    // Проверка обновления статуса сообщения

    [Fact]
    public void UpdateStatus_ChangesMessageStatus()
    {
        var store = new MessageStore();


        var packet = new MessagePacket
        {
            MessageId = Guid.NewGuid(),

            Payload = new byte[]
            {
            1, 2, 3
            },

            Status = MessageStatus.Sent
        };


        store.Add(packet);


        store.UpdateStatus(
            packet.MessageId,
            MessageStatus.Delivered
        );


        var result =
            store.TryGet(
                packet.MessageId,
                out var updated
            );


        Assert.True(result);


        Assert.Equal(
            MessageStatus.Delivered,
            updated.Status
        );
    }

    // Проверка поиска несуществующего сообщения

    [Fact]
    public void TryGet_UnknownMessageId_ReturnsFalse()
    {
        var store = new MessageStore();


        var result =
            store.TryGet(
                Guid.NewGuid(),
                out var message
            );


        Assert.False(result);
    }
}