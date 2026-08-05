using OfflineMessenger.Core;
using OfflineMessenger.Crypto;
using OfflineMessenger.Transport.Memory;
using Xunit;

namespace OfflineMessenger.Tests;

public class ChatEngineTests
{
    // Проверка успешного handshake между двумя движками

    [Fact]
    public async Task ChatEngines_CompleteHandshake()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();

        await engineB.WaitForHandshakeAsync();


        Assert.True(true);
    }

    // Проверка отправки и получения сообщения после handshake

    [Fact]
    public async Task SendMessage_AfterHandshake_MessageIsReceived()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();


        string? receivedMessage = null;


        engineB.MessageReceived += message =>
        {
            receivedMessage = message;
        };


        await engineA.SendMessageAsync(
            "Hello"
        );


        Assert.Equal(
            "Hello",
            receivedMessage
        );
    }

    // Проверка уникальности идентификаторов сообщений

    [Fact]
    public async Task SendMessage_CreatesUniqueMessageIds()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();


        var firstId =
            await engineA.SendMessageAsync(
                "First"
            );


        var secondId =
            await engineA.SendMessageAsync(
                "Second"
            );


        Assert.NotEqual(
            firstId,
            secondId
        );
    }

    // Проверка запрета отправки до handshake

    [Fact]
    public async Task SendMessage_BeforeHandshake_ThrowsException()
    {
        var transport = new InMemoryTransport();

        var crypto = new CryptoService();


        var engine = new ChatEngine(
            transport,
            crypto
        );


        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await engine.SendMessageAsync(
                    "Hello"
                );
            });
    }

    // Проверка однократного вызова события получения сообщения

    [Fact]
    public async Task ReceiveMessage_MessageReceivedCalledOnce()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();


        var receivedCount = 0;


        engineB.MessageReceived += message =>
        {
            receivedCount++;
        };


        await engineA.SendMessageAsync(
            "Hello"
        );


        Assert.Equal(
            1,
            receivedCount
        );
    }

    // Проверка изоляции разных чатов

    [Fact]
    public async Task SeparateChatEngines_DoNotReceiveForeignMessages()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();

        var transportC = new InMemoryTransport();
        var transportD = new InMemoryTransport();


        transportA.ConnectTo(transportB);
        transportC.ConnectTo(transportD);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );

        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        var engineC = new ChatEngine(
            transportC,
            crypto
        );

        var engineD = new ChatEngine(
            transportD,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();

        await engineC.WaitForHandshakeAsync();
        await engineD.WaitForHandshakeAsync();


        string? receivedByB = null;
        string? receivedByD = null;


        engineB.MessageReceived += message =>
        {
            receivedByB = message;
        };


        engineD.MessageReceived += message =>
        {
            receivedByD = message;
        };


        await engineA.SendMessageAsync(
            "Hello"
        );


        Assert.Equal(
            "Hello",
            receivedByB
        );


        Assert.Null(
            receivedByD
        );
    }

    // Проверка передачи Unicode-сообщений

    [Fact]
    public async Task SendMessage_UnicodeText_IsReceivedCorrectly()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();


        string? received = null;


        engineB.MessageReceived += message =>
        {
            received = message;
        };


        var text =
            "Привет 👋 Сербия 🇷🇸";


        await engineA.SendMessageAsync(
            text
        );


        Assert.Equal(
            text,
            received
        );
    }

    // Проверка порядка нескольких сообщений

    [Fact]
    public async Task SendMultipleMessages_MessagesReceivedInOrder()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();


        var received = new List<string>();


        engineB.MessageReceived += message =>
        {
            received.Add(message);
        };


        await engineA.SendMessageAsync(
            "First"
        );


        await engineA.SendMessageAsync(
            "Second"
        );


        Assert.Equal(
            2,
            received.Count
        );


        Assert.Equal(
            "First",
            received[0]
        );


        Assert.Equal(
            "Second",
            received[1]
        );
    }

    // Проверка передачи длинного сообщения

    [Fact]
    public async Task SendLongMessage_MessageIsReceivedCorrectly()
    {
        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();


        transportA.ConnectTo(transportB);


        var crypto = new CryptoService();


        var engineA = new ChatEngine(
            transportA,
            crypto
        );


        var engineB = new ChatEngine(
            transportB,
            crypto
        );


        await engineA.WaitForHandshakeAsync();
        await engineB.WaitForHandshakeAsync();


        string? received = null;


        engineB.MessageReceived += message =>
        {
            received = message;
        };


        var longMessage =
            new string(
                'A',
                10000
            );


        await engineA.SendMessageAsync(
            longMessage
        );


        Assert.Equal(
            longMessage,
            received
        );
    }

    // Проверка защиты от изменения зашифрованных данных

    [Fact]
    public async Task TamperedEncryptedMessage_ThrowsException()
    {
        var crypto = new CryptoService();


        var key =
            System.Security.Cryptography.RandomNumberGenerator
                .GetBytes(32);


        var data =
            System.Text.Encoding.UTF8.GetBytes(
                "Secret message"
            );


        var encrypted =
            crypto.Encrypt(
                key,
                data,
                out var nonce,
                out var tag
            );


        // Меняем один байт шифротекста
        encrypted[0] ^= 0xFF;


        Assert.ThrowsAny<
            System.Security.Cryptography.CryptographicException
        >(
            () =>
            {
                crypto.Decrypt(
                    key,
                    encrypted,
                    nonce,
                    tag
                );
            });
    }
}