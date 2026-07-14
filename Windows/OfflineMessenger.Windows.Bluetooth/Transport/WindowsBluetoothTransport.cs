using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OfflineMessenger.Bluetooth.Windows.Transport
{
    public class WindowsBluetoothTransport
    {
        private StreamSocket? socket;
        private DataWriter? writer;
        private DataReader? reader;


        public async Task<DeviceInformation?> DiscoverAsync()
        {
            var selector =
                RfcommDeviceService.GetDeviceSelector(
                    RfcommServiceId.SerialPort);

            var devices =
                await DeviceInformation.FindAllAsync(selector);


            foreach (var device in devices)
            {
                if (device.Name == "OfflineMessenger")
                {
                    return device;
                }
            }

            return null;
        }


        public async Task ConnectAsync(DeviceInformation device)
        {
            var service =
                await RfcommDeviceService.FromIdAsync(device.Id);


            if (service == null)
            {
                throw new Exception(
                    "RFCOMM service not found");
            }


            socket = new StreamSocket();


            await socket.ConnectAsync(
                service.ConnectionHostName,
                service.ConnectionServiceName);


            writer =
                new DataWriter(socket.OutputStream);


            reader =
                new DataReader(socket.InputStream);


            reader.InputStreamOptions =
                InputStreamOptions.Partial;


            Console.WriteLine("Bluetooth connected");
        }


        public async Task SendAsync(byte[] data)
        {
            if (writer == null)
            {
                throw new Exception("Not connected");
            }

            writer.WriteBytes(data);

            await writer.StoreAsync();
            await writer.FlushAsync();
        }


        public async Task<byte[]?> ReceiveAsync()
        {
            if (reader == null)
            {
                throw new Exception("Not connected");
            }

            uint size = await reader.LoadAsync(1024);

            if (size == 0)
            {
                return null;
            }

            byte[] buffer = new byte[size];

            reader.ReadBytes(buffer);

            return buffer;
        }
    }
}