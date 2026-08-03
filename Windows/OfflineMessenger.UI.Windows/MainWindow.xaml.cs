using OfflineMessenger.Bluetooth.Windows.Transport;
using OfflineMessenger.Core;
using OfflineMessenger.Crypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;


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


        Loaded += MainWindow_Loaded;
    }



    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await _bluetoothTransport.ConnectAsync("");


            Debug.WriteLine(
                "Bluetooth connected"
            );


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
                    }
                });
            };



            _bluetoothChat.StatusChanged += status =>
            {
                Debug.WriteLine(
                    $"STATUS: {status}"
                );
            };



            _bluetoothChat.DebugMessage += message =>
            {
                Debug.WriteLine(
                    message
                );
            };



            _bluetoothChat.MessageReceived += message =>
            {
                Dispatcher.Invoke(() =>
                {
                    var item = new ChatItem
                    {
                        Id = Guid.NewGuid(),
                        Text = message,
                        Status = "✓ Delivered",
                        IsMine = false
                    };

                    Debug.WriteLine(
    $"ADDING INCOMING MESSAGE: {item.Text}, IsMine={item.IsMine}"
);

                    ChatList.Items.Add(item);
                });
            };



            await _bluetoothChat.WaitForHandshakeAsync();


            Debug.WriteLine(
                "Handshake completed"
            );

        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                ex.ToString()
            );
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
            Status = "⏳ Sending",
            IsMine = true
        };

        Debug.WriteLine(
    $"ADDING OWN MESSAGE: {item.Text}, IsMine={item.IsMine}"
);


        _uiMessages[messageId] = item;


        ChatList.Items.Add(item);


        MessageInput.Clear();
    }
}