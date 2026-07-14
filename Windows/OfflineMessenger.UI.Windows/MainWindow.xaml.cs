using OfflineMessenger.Core;
using OfflineMessenger.Crypto;
using OfflineMessenger.Transport.Memory;
using OfflineMessenger.Bluetooth.Windows.Transport;

using System;
using System.Windows;


namespace OfflineMessenger.UI.Windows;


public partial class MainWindow : Window
{
    private readonly BluetoothTransport _bluetoothTransport;

    private ChatEngine? _bluetoothChat;


    // Старый тестовый режим оставляем
    private readonly ChatEngine _chatA;
    private readonly ChatEngine _chatB;



    public MainWindow()
    {
        InitializeComponent();


        // =========================
        // REAL BLUETOOTH TRANSPORT
        // =========================

        _bluetoothTransport = new BluetoothTransport();

        _bluetoothTransport.DataReceived += data =>
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    $"Bluetooth received: {data.Length} bytes");
            });
        };

        // =========================
        // SIMULATED DEVICES TEST
        // =========================

        var transportA = new InMemoryTransport();
        var transportB = new InMemoryTransport();

        transportA.ConnectTo(transportB);


        _chatA = new ChatEngine(
            transportA,
            new CryptoService());


        _chatB = new ChatEngine(
            transportB,
            new CryptoService());



        _chatA.MessageReceived += msg =>
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    "A received: " + msg);
            });
        };


        _chatB.MessageReceived += msg =>
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    "B received: " + msg);
            });
        };


        Loaded += MainWindow_Loaded;
    }



    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            // Сначала реальный Bluetooth
            await _bluetoothTransport.ConnectAsync("");


            MessageBox.Show(
                "Bluetooth connected");



            // Только после подключения создаём ChatEngine

            _bluetoothChat = new ChatEngine(
            _bluetoothTransport,
            new CryptoService());

            _bluetoothChat.StartHandshake();

            await _bluetoothChat.WaitForHandshakeAsync();

            await _bluetoothChat.SendMessageAsync(
                "HELLO FROM UI WINDOWS");

           
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Bluetooth error: " + ex.Message);
        }


    }
}