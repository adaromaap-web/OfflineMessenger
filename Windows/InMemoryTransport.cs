using OfflineMessenger.Transport.Abstractions;
using System;

namespace OfflineMessenger.Transport.Memory;

public class InMemoryTransport : ITransport
{
    public event Action<byte[]>? DataReceived;

    private InMemoryTransport? _peer;

    public bool IsConnected => _peer != null;

    // =========================
    // CONNECT TWO VIRTUAL DEVICES
    // =========================
    public void ConnectTo(InMemoryTransport other)
    {
        _peer = other;
        other._peer = this;
    }

    // =========================
    // SERVER SIDE (not used in memory mode)
    // =========================
    public Task StartServerAsync()
    {
        return Task.CompletedTask;
    }

    // =========================
    // CLIENT SIDE (not used in memory mode)
    // =========================
    public Task ConnectAsync(string deviceAddress)
    {
        return Task.CompletedTask;
    }

    // =========================
    // SEND DATA TO PEER
    // =========================
    public Task SendAsync(byte[] data)
    {
        if (_peer == null)
            throw new InvalidOperationException("Not connected");

        // мгновенная доставка без Bluetooth/сети
        _peer.DataReceived?.Invoke(data);

        return Task.CompletedTask;
    }
}