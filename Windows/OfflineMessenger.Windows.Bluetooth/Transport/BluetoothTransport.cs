using OfflineMessenger.Transport.Abstractions;
using System;
using System.Buffers.Binary;
using System.Threading.Tasks;
using System.Windows;

namespace OfflineMessenger.Bluetooth.Windows.Transport
{
    public class BluetoothTransport : ITransport
    {
        private readonly WindowsBluetoothTransport bluetooth;

        public event Action<byte[]>? DataReceived;
        public event Action<string>? DebugMessage;



        public BluetoothTransport()
        {
            bluetooth = new WindowsBluetoothTransport();

            bluetooth.DebugMessage += message =>
            {
                DebugMessage?.Invoke(message);
            };
        }


        public async Task ConnectAsync(string deviceId)
        {
            var device =
                await bluetooth.DiscoverAsync();


            if (device == null)
            {
                throw new Exception(
                    "Bluetooth device not found");
            }


            await bluetooth.ConnectAsync(device);


            _ = ReceiveLoop();
        }



        public async Task SendAsync(byte[] data)
        {
            var size = new byte[4];

            BinaryPrimitives.WriteInt32BigEndian(
                size,
                data.Length
            );


            var framed =
                new byte[4 + data.Length];


            Array.Copy(
                size,
                0,
                framed,
                0,
                4
            );


            Array.Copy(
                data,
                0,
                framed,
                4,
                data.Length
            );


            DebugMessage?.Invoke(
                $"Windows sending framed packet {framed.Length} bytes"
            );


            await bluetooth.SendAsync(framed);
        }


        private async Task ReceiveLoop()
        {
            try
            {
                while (true)
                {
                    var data =
                        await bluetooth.ReceiveAsync();


                    if (data == null)
                    {
                        break;
                    }


                    if (data.Length < 4)
                    {
                        continue;
                    }


                    int size =
                        BinaryPrimitives.ReadInt32BigEndian(
                            data.AsSpan(0, 4)
                        );


                    if (size <= 0 || size > 10_000_000)
                    {
                        continue;
                    }


                    if (data.Length - 4 != size)
                    {
                        DebugMessage?.Invoke(
                            $"Invalid packet size. Expected {size}, received {data.Length - 4}"
                        );

                        continue;
                    }


                    var packet =
                        new byte[size];


                    Array.Copy(
                        data,
                        4,
                        packet,
                        0,
                        size
                    );


                    DebugMessage?.Invoke(
                        $"Windows received packet {packet.Length} bytes"
                    );


                    DataReceived?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
                DebugMessage?.Invoke(
                    "Bluetooth receive error: " + ex.Message
                );
            }
        }
    }
}