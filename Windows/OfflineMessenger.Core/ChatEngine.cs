using OfflineMessenger.Core.Protocol;
using OfflineMessenger.Crypto;
using OfflineMessenger.Transport.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Core;

public class ChatEngine
{
    private readonly ITransport _transport;
    private readonly KeyExchangeService _keyExchange = new();

    private KeyPair _keyPair;
    private byte[] _remotePublicKey;

    private byte[] _sharedSecret;
    private byte[] _sessionKey;

    private readonly Guid _sessionId = Guid.NewGuid();

    public event Action<string> MessageReceived;

    private bool _handshakeCompleted = false;
    private bool _isInitiator = false;

    private bool _handshakeStarted = false;

    public ChatEngine(ITransport transport, ICryptoService crypto)
    {
        _transport = transport;
        _transport.DataReceived += OnDataReceived;
        StartHandshake();
    }

    // =========================
    // START HANDSHAKE (INITIATOR)
    // =========================
    public async void StartHandshake()
    {
        if (_handshakeStarted)
            return;

        _handshakeStarted = true;

        _keyPair = _keyExchange.GenerateKeyPair();

        var packet = new HandshakePacket
        {
            Type = MessageType.HandshakeInit,
            PublicKey = _keyPair.PublicKey
        };

        await _transport.SendAsync(PacketSerializer.SerializeHandshake(packet));
    }

    // =========================
    // SEND MESSAGE
    // =========================
    public Task SendMessageAsync(string message)
    {
        if (!_handshakeCompleted)
            throw new InvalidOperationException("Handshake not completed");

        var plainBytes = Encoding.UTF8.GetBytes(message);

        var encrypted = new CryptoService().Encrypt(
            _sessionKey,
            plainBytes,
            out var nonce,
            out var tag
        );

        var packet = new MessagePacket
        {
            Type = MessageType.Chat,
            SessionId = _sessionId,
            MessageId = Guid.NewGuid(),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Payload = encrypted,
            Nonce = nonce,
            Tag = tag
        };

        return _transport.SendAsync(PacketSerializer.Serialize(packet));
    }

    // =========================
    // RECEIVE DATA
    // =========================
    private void OnDataReceived(byte[] data)
    {
        if (!_handshakeStarted)
            return;
        var type = (MessageType)data[0];

        // ---------------- HANDSHAKE INIT ----------------
        if (type == MessageType.HandshakeInit)
        {
            var packet = PacketSerializer.DeserializeHandshake(data);

            _remotePublicKey = packet.PublicKey;

            // responder генерирует свой ключ
            _keyPair = _keyExchange.GenerateKeyPair();

            var reply = new HandshakePacket
            {
                Type = MessageType.HandshakeReply,
                PublicKey = _keyPair.PublicKey
            };

            _transport.SendAsync(PacketSerializer.SerializeHandshake(reply));

            TryCompleteHandshake();
            return;
        }

        // ---------------- HANDSHAKE REPLY ----------------
        if (type == MessageType.HandshakeReply)
        {
            var packet = PacketSerializer.DeserializeHandshake(data);

            _remotePublicKey = packet.PublicKey;

            TryCompleteHandshake();
            return;
        }

        // ---------------- CHAT ----------------
        if (!_handshakeCompleted)
            return;

        var chatPacket = PacketSerializer.Deserialize(data);

        if (chatPacket.Type != MessageType.Chat)
            return;

        var decrypted = new CryptoService().Decrypt(
            _sessionKey,
            chatPacket.Payload,
            chatPacket.Nonce,
            chatPacket.Tag
        );

        var message = Encoding.UTF8.GetString(decrypted);

        MessageReceived?.Invoke(message);
    }

    // =========================
    // COMPLETE HANDSHAKE
    // =========================
    private void TryCompleteHandshake()
    {
        if (_sharedSecret != null)
            return;

        if (_remotePublicKey == null || _keyPair == null)
            return;

        _sharedSecret = _keyExchange.DeriveSharedSecret(
            _keyPair.PrivateKey,
            _remotePublicKey
        );

        _sessionKey = Hkdf.DeriveKey(_sharedSecret);

        _handshakeCompleted = true;
    }

    public Task WaitForHandshakeAsync()
    {
        return Task.Run(() =>
        {
            while (!_handshakeCompleted)
            {
                Task.Delay(50).Wait();
            }
        });
    }
}