using System;
using System.Buffers.Binary;
using System.Threading.Tasks;
using OfflineMessenger.Transport.Abstractions;

namespace OfflineMessenger.Bluetooth.Windows.Transport
{
    public class BluetoothTransport : ITransport
    {
        private readonly WindowsBluetoothTransport bluetooth;

        public event Action<byte[]>? DataReceived;


        public BluetoothTransport()
        {
            bluetooth = new WindowsBluetoothTransport();
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
            // 4 байта длины
            var length =
                new byte[4];


            BinaryPrimitives.WriteInt32BigEndian(
                length,
                data.Length
            );


            await bluetooth.SendAsync(length);

            await bluetooth.SendAsync(data);
        }



        private async Task ReceiveLoop()
        {
            try
            {
                while (true)
                {
                    // сначала читаем длину
                    var lengthBytes =
                        await bluetooth.ReceiveAsync();


                    if (lengthBytes == null)
                    {
                        break;
                    }


                    if (lengthBytes.Length != 4)
                    {
                        continue;
                    }


                    int size =
                        BinaryPrimitives.ReadInt32BigEndian(
                            lengthBytes
                        );


                    if (size <= 0 || size > 10_000_000)
                    {
                        continue;
                    }



                    // потом читаем сам пакет
                    var data =
                        await bluetooth.ReceiveAsync();


                    if (data == null)
                    {
                        break;
                    }

           

                    DataReceived?.Invoke(data);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Bluetooth receive error: "
                    + ex.Message);
            }
        }
    }
}