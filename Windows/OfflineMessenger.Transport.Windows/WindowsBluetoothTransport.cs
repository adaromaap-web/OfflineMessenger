using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using OfflineMessenger.Transport.Abstractions;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OfflineMessenger.Transport.Windows;

public class WindowsBluetoothTransport : ITransport
{
    public event Action<byte[]>? DataReceived;
    public event Action? ConnectionReady;

    private BluetoothListener? _listener;
    private BluetoothClient? _client;
    private NetworkStream? _stream;

    public bool IsConnected => _stream != null;

    // =========================
    // SERVER SIDE
    // =========================
    public async Task StartServerAsync()
    {
        _listener = new BluetoothListener(BluetoothConstants.ServiceId);
        _listener.Start();

        _client = await _listener.AcceptBluetoothClientAsync();

        _stream = _client.GetStream();

        StartReceiveLoop();

        ConnectionReady?.Invoke();
    }

    // =========================
    // CLIENT SIDE
    // =========================
    public async Task ConnectAsync(string deviceAddress)
    {
        var client = new BluetoothClient();

        var ep = new BluetoothEndPoint(
            BluetoothAddress.Parse(deviceAddress),
            BluetoothConstants.ServiceId
        );

        await Task.Run(() =>
        {
            client.Connect(ep);
        });

        _client = client;
        _stream = _client.GetStream();

        StartReceiveLoop();

        ConnectionReady?.Invoke();
    }

    // =========================
    // RECEIVE LOOP
    // =========================
    private void StartReceiveLoop()
    {
        Task.Run(async () =>
        {
            var buffer = new byte[4096];

            while (_stream != null)
            {
                try
                {
                    var read = await _stream.ReadAsync(buffer, 0, buffer.Length);

                    if (read > 0)
                    {
                        var data = new byte[read];
                        Array.Copy(buffer, data, read);

                        DataReceived?.Invoke(data);
                    }
                }
                catch
                {
                    break;
                }
            }
        });
    }

    // =========================
    // SEND
    // =========================
    public async Task SendAsync(byte[] data)
    {
        if (_stream == null)
            throw new InvalidOperationException("Not connected");

        await _stream.WriteAsync(data, 0, data.Length);
        await _stream.FlushAsync();
    }
}