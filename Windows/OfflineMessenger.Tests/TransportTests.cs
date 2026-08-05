using OfflineMessenger.Transport.Memory;
using Xunit;

namespace OfflineMessenger.Tests;

public class TransportTests
{
    // Проверка доставки данных через транспорт

    [Fact]
    public async Task SendAsync_MessageIsReceivedByPeer()
    {
        var sender = new InMemoryTransport();
        var receiver = new InMemoryTransport();


        sender.ConnectTo(receiver);


        byte[]? received = null;


        receiver.DataReceived += data =>
        {
            received = data;
        };


        var message = new byte[]
        {
            1, 2, 3, 4
        };


        await sender.SendAsync(message);


        Assert.Equal(
            message,
            received
        );
    }

    // Проверка порядка доставки сообщений

    [Fact]
    public async Task SendAsync_MultipleMessages_PreservesOrder()
    {
        var sender = new InMemoryTransport();
        var receiver = new InMemoryTransport();


        sender.ConnectTo(receiver);


        var received = new List<byte[]>();


        receiver.DataReceived += data =>
        {
            received.Add(data);
        };


        await sender.SendAsync(
            new byte[] { 1 }
        );

        await sender.SendAsync(
            new byte[] { 2 }
        );

        await sender.SendAsync(
            new byte[] { 3 }
        );


        Assert.Equal(
            3,
            received.Count
        );


        Assert.Equal(
            new byte[] { 1 },
            received[0]
        );


        Assert.Equal(
            new byte[] { 2 },
            received[1]
        );


        Assert.Equal(
            new byte[] { 3 },
            received[2]
        );
    }

    // Проверка ошибки отправки без подключения

    [Fact]
    public async Task SendAsync_WithoutConnection_ThrowsException()
    {
        var transport = new InMemoryTransport();


        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await transport.SendAsync(
                    new byte[]
                    {
                    1, 2, 3
                    }
                );
            });
    }

    // Проверка изоляции соединений

    [Fact]
    public async Task SendAsync_SeparateConnections_DoNotMixMessages()
    {
        var a = new InMemoryTransport();
        var b = new InMemoryTransport();

        var c = new InMemoryTransport();
        var d = new InMemoryTransport();


        a.ConnectTo(b);
        c.ConnectTo(d);


        byte[]? receivedByB = null;
        byte[]? receivedByD = null;


        b.DataReceived += data =>
        {
            receivedByB = data;
        };


        d.DataReceived += data =>
        {
            receivedByD = data;
        };


        await a.SendAsync(
            new byte[]
            {
            1
            }
        );


        await c.SendAsync(
            new byte[]
            {
            2
            }
        );


        Assert.Equal(
            new byte[] { 1 },
            receivedByB
        );


        Assert.Equal(
            new byte[] { 2 },
            receivedByD
        );
    }
}