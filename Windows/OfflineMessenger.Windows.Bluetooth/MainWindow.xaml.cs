using System;
using System.Windows;
using OfflineMessenger.Bluetooth.Windows.Transport;

namespace OfflineMessenger.Windows.Bluetooth
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var transport =
                    new WindowsBluetoothTransport();


                var device =
                    await transport.DiscoverAsync();


                if (device == null)
                {
                    MessageBox.Show(
                        "Device not found");

                    return;
                }


                MessageBox.Show(
                    $"Found: {device.Name}");


                await transport.ConnectAsync(device);


                MessageBox.Show(
                    "Connected");


                await transport.SendAsync(
    System.Text.Encoding.UTF8.GetBytes("HELLO FROM WINDOWS"));


                MessageBox.Show(
                    "Message sent");


                var response = await transport.ReceiveAsync();

                if (response != null)
                {
                    MessageBox.Show(
                        System.Text.Encoding.UTF8.GetString(response));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message);
            }
        }
    }
}