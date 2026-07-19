using OfflineMessenger.Bluetooth.Windows.Transport;
using OfflineMessenger.Core;
using OfflineMessenger.Crypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace OfflineMessenger.UI.Windows;


public partial class MainWindow : Window
{
    private readonly BluetoothTransport _bluetoothTransport;

    private ChatEngine? _bluetoothChat;

    private readonly Dictionary<Guid, ChatItem> _uiMessages = new();


    public MainWindow()
    {
        InitializeComponent();


        SendButton.Click += SendButton_Click;


        _bluetoothTransport = new BluetoothTransport();


        _bluetoothTransport.DebugMessage += message =>
        {
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(
                    message + Environment.NewLine);

                LogBox.ScrollToEnd();
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
            await _bluetoothTransport.ConnectAsync("");


            LogBox.AppendText(
                "Bluetooth connected" +
                Environment.NewLine);



            _bluetoothChat = new ChatEngine(
                _bluetoothTransport,
                new CryptoService()
            );



            _bluetoothChat.MessageStatusChanged += (id, status) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (_uiMessages.TryGetValue(
                            id,
                            out var item))
                    {
                        item.Status = "✓ Delivered";

                        LogBox.ScrollToEnd();
                    }
                });
            };



            _bluetoothChat.StatusChanged += status =>
            {
                Dispatcher.Invoke(() =>
                {
                    LogBox.AppendText(
                        status + Environment.NewLine);

                    LogBox.ScrollToEnd();
                });
            };



            _bluetoothChat.DebugMessage += message =>
            {
                Dispatcher.Invoke(() =>
                {
                    LogBox.AppendText(
                        message + Environment.NewLine);

                    LogBox.ScrollToEnd();
                });
            };



            _bluetoothChat.MessageReceived += message =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatList.Items.Add(
                        new ChatItem
                        {
                            Id = Guid.NewGuid(),
                            Text = message,
                            Status = "✓ Delivered"
                        }
                    );
                });
            };


            await _bluetoothChat.WaitForHandshakeAsync();



            LogBox.AppendText(
                "Handshake completed" +
                Environment.NewLine);

        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Bluetooth error: " + ex.Message);
        }
    }



    private async void SendButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_bluetoothChat == null)
            return;


        var text = MessageInput.Text;


        if (string.IsNullOrWhiteSpace(text))
            return;



        var messageId =
            await _bluetoothChat.SendMessageAsync(text);



        var item = new ChatItem
        {
            Id = messageId,
            Text = text,
            Status = "⏳ Sending"
        };



        _uiMessages[messageId] = item;



        ChatList.Items.Add(item);



        MessageInput.Clear();
    }
}