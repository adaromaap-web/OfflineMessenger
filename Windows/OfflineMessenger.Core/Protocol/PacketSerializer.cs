using System;
using System.IO;
using System.Buffers.Binary;

namespace OfflineMessenger.Core.Protocol;

public static class PacketSerializer
{
    public static byte[] Serialize(MessagePacket packet)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        // 🔹 HEADER
        bw.Write((byte)packet.Type);
        bw.Write(GuidToBytes(packet.SessionId));
        bw.Write(GuidToBytes(packet.MessageId));

        var timestamp =
            new byte[8];

        BinaryPrimitives.WriteInt64BigEndian(
            timestamp,
            packet.Timestamp
        );

        bw.Write(timestamp);

        // Nonce
        var nonceLength = new byte[4];

        BinaryPrimitives.WriteInt32BigEndian(
            nonceLength,
            packet.Nonce.Length
        );

        bw.Write(nonceLength);
        bw.Write(packet.Nonce);


        // Tag
        var tagLength = new byte[4];

        BinaryPrimitives.WriteInt32BigEndian(
            tagLength,
            packet.Tag.Length
        );

        bw.Write(tagLength);
        bw.Write(packet.Tag);


        // Payload
        var payloadLength = new byte[4];

        BinaryPrimitives.WriteInt32BigEndian(
            payloadLength,
            packet.Payload.Length
        );

        bw.Write(payloadLength);
        bw.Write(packet.Payload);

        return ms.ToArray();
    }

    public static MessagePacket Deserialize(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);

        var packet = new MessagePacket();

        // 🔹 HEADER
        packet.Type = (MessageType)br.ReadByte();
        packet.SessionId = BytesToGuid(
        br.ReadBytes(16)
            );
        packet.MessageId =
            BytesToGuid(
                br.ReadBytes(16)
            );


        packet.Timestamp =
            BinaryPrimitives.ReadInt64BigEndian(
                br.ReadBytes(8)
            );
        packet.Timestamp = br.ReadInt64();

        // 🔹 Nonce
        int nonceLen =
            BinaryPrimitives.ReadInt32BigEndian(
                br.ReadBytes(4)
            );

        packet.Nonce =
            br.ReadBytes(nonceLen);


        // 🔹 Tag
        int tagLen =
            BinaryPrimitives.ReadInt32BigEndian(
                br.ReadBytes(4)
            );

        packet.Tag =
            br.ReadBytes(tagLen);


        // 🔹 Payload
        int payloadLen =
            BinaryPrimitives.ReadInt32BigEndian(
                br.ReadBytes(4)
            );

        packet.Payload =
            br.ReadBytes(payloadLen);

        return packet;
    }

    public static byte[] SerializeHandshake(HandshakePacket packet)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.Write((byte)packet.Type);

        bw.Write(packet.PublicKey.Length);
        bw.Write(packet.PublicKey);

        return ms.ToArray();
    }

    public static HandshakePacket DeserializeHandshake(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);

        var packet = new HandshakePacket();

        packet.Type = (MessageType)br.ReadByte();

        int len = br.ReadInt32();
        packet.PublicKey = br.ReadBytes(len);

        return packet;
    }

    private static byte[] GuidToBytes(Guid guid)
    {
        var result = new byte[16];

        var bytes = guid.ToByteArray();

        Buffer.BlockCopy(
            bytes,
            0,
            result,
            0,
            16
        );

        return result;
    }

    private static Guid BytesToGuid(byte[] bytes)
    {
        return new Guid(bytes);
    }
}