using OfflineMessenger.Core.Protocol;
using Xunit;

namespace OfflineMessenger.Tests;

public class ProtocolTests
{
    // Проверка сериализации MessagePacket

    [Fact]
    public void MessagePacket_SerializeDeserialize_PreservesData()
    {
        var packet = new MessagePacket
        {
            Type = MessageType.Chat,

            SessionId = Guid.NewGuid(),

            MessageId = Guid.NewGuid(),

            Timestamp = 123456789,

            Payload = new byte[]
            {
            1,2,3,4,5
            },

            Nonce = new byte[]
            {
            10,11,12
            },

            Tag = new byte[]
            {
            20,21,22
            }
        };


        var bytes =
            PacketSerializer.Serialize(packet);


        var restored =
            PacketSerializer.Deserialize(bytes);


        Assert.Equal(
            packet.Type,
            restored.Type
        );


        Assert.Equal(
            packet.SessionId,
            restored.SessionId
        );


        Assert.Equal(
            packet.MessageId,
            restored.MessageId
        );


        Assert.Equal(
            packet.Timestamp,
            restored.Timestamp
        );


        Assert.Equal(
            packet.Payload,
            restored.Payload
        );


        Assert.Equal(
            packet.Nonce,
            restored.Nonce
        );


        Assert.Equal(
            packet.Tag,
            restored.Tag
        );
    }

    // Проверка сериализации HandshakePacket

    [Fact]
    public void HandshakePacket_SerializeDeserialize_PreservesPublicKey()
    {
        var packet = new HandshakePacket
        {
            Type = MessageType.HandshakeInit,

            PublicKey = new byte[]
            {
            1, 2, 3, 4, 5, 6, 7, 8
            }
        };


        var bytes =
            PacketSerializer.SerializeHandshake(packet);


        var restored =
            PacketSerializer.DeserializeHandshake(bytes);


        Assert.Equal(
            packet.Type,
            restored.Type
        );


        Assert.Equal(
            packet.PublicKey,
            restored.PublicKey
        );
    }

    // Проверка сериализации AckPacket

    [Fact]
    public void AckPacket_SerializeDeserialize_PreservesData()
    {
        var packet = new AckPacket
        {
            SessionId = Guid.NewGuid(),

            MessageId = Guid.NewGuid(),

            Received = true
        };


        var bytes =
            PacketSerializer.SerializeAck(packet);


        var restored =
            PacketSerializer.DeserializeAck(bytes);


        Assert.Equal(
            packet.SessionId,
            restored.SessionId
        );


        Assert.Equal(
            packet.MessageId,
            restored.MessageId
        );


        Assert.Equal(
            packet.Received,
            restored.Received
        );
    }

    // Проверка сохранения типа сообщения

    [Fact]
    public void MessagePacket_SerializeDeserialize_PreservesMessageType()
    {
        var types = new[]
        {
        MessageType.Chat,
        MessageType.HandshakeInit,
        MessageType.HandshakeReply
    };


        foreach (var type in types)
        {
            var packet = new MessagePacket
            {
                Type = type,

                SessionId = Guid.NewGuid(),

                MessageId = Guid.NewGuid(),

                Timestamp = 123,

                Payload = new byte[]
                {
                1, 2, 3
                },

                Nonce = new byte[]
                {
                4, 5, 6
                },

                Tag = new byte[]
                {
                7, 8, 9
                }
            };


            var bytes =
                PacketSerializer.Serialize(packet);


            var restored =
                PacketSerializer.Deserialize(bytes);


            Assert.Equal(
                type,
                restored.Type
            );
        }
    }

    // Проверка больших сообщений

    [Fact]
    public void MessagePacket_SerializeDeserialize_PreservesLargePayload()
    {
        var payload = new byte[10000];

        Random.Shared.NextBytes(payload);


        var packet = new MessagePacket
        {
            Type = MessageType.Chat,

            SessionId = Guid.NewGuid(),

            MessageId = Guid.NewGuid(),

            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),

            Payload = payload,

            Nonce = new byte[]
            {
            1,2,3,4,5,6,7,8,9,10,11,12
            },

            Tag = new byte[]
            {
            1,2,3,4,5,6,7,8,
            9,10,11,12,13,14,15,16
            }
        };


        var bytes =
            PacketSerializer.Serialize(packet);


        var restored =
            PacketSerializer.Deserialize(bytes);


        Assert.Equal(
            payload,
            restored.Payload
        );
    }

    

}