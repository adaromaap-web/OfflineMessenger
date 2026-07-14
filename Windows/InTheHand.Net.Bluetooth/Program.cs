using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Text;
using System.Linq;

class Program
{
    static void Main()
    {
        var client = new BluetoothClient();

        Console.WriteLine("Searching devices...");

        var devices = client.DiscoverDevices();

        var phone = devices.FirstOrDefault(d =>
            d.DeviceName != null &&
            d.DeviceName.ToLower().Contains("android") // или имя телефона
        );

        if (phone == null)
        {
            Console.WriteLine("Phone not found");
            return;
        }

        Console.WriteLine("Found: " + phone.DeviceName);

        client.Connect(phone.DeviceAddress, BluetoothService.SerialPort);

        Console.WriteLine("Connected!");

        var stream = client.GetStream();

        var message = "HELLO FROM WINDOWS";
        var data = Encoding.UTF8.GetBytes(message);

        stream.Write(data, 0, data.Length);

        Console.WriteLine("Sent!");
    }
}