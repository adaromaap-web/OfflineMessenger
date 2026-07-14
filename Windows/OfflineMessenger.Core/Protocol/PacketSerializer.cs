using System.IO;

namespace OfflineMessenger.Core.Protocol;

public static class PacketSerializer
{
    public static byte[] Serialize(MessagePacket packet)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        // 🔹 HEADER
        bw.Write((byte)packet.Type);
        bw.Write(packet.SessionId.ToByteArray());
        bw.Write(packet.MessageId.ToByteArray());
        bw.Write(packet.Timestamp);

        // 🔹 Nonce
        bw.Write(packet.Nonce.Length);
        bw.Write(packet.Nonce);

        // 🔹 Tag
        bw.Write(packet.Tag.Length);
        bw.Write(packet.Tag);

        // 🔹 Payload
        bw.Write(packet.Payload.Length);
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
        packet.SessionId = new Guid(br.ReadBytes(16));
        packet.MessageId = new Guid(br.ReadBytes(16));
        packet.Timestamp = br.ReadInt64();

        // 🔹 Nonce
        int nonceLen = br.ReadInt32();
        packet.Nonce = br.ReadBytes(nonceLen);

        // 🔹 Tag
        int tagLen = br.ReadInt32();
        packet.Tag = br.ReadBytes(tagLen);

        // 🔹 Payload
        int payloadLen = br.ReadInt32();
        packet.Payload = br.ReadBytes(payloadLen);

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
}