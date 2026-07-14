namespace OfflineMessenger.Core.Protocol;

public class HandshakePacket
{
    public MessageType Type { get; set; }
    public byte[] PublicKey { get; set; }
}