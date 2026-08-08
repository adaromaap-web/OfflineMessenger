
using OfflineMessenger.Bluetooth.Windows.Transport;
using OfflineMessenger.Core;
using OfflineMessenger.Crypto;
using OfflineMessenger.Core.MessageHistory;
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

    private readonly MessageRepository _messageRepository;


    public MainWindow()
    {
        InitializeComponent();

        SendButton.Click += SendButton_Click;

        _bluetoothTransport = new BluetoothTransport();

        _messageRepository =
            new MessageRepository(
                MessageDatabasePath.GetPath());

        Loaded += MainWindow_Loaded;
    }


    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await _messageRepository.InitializeAsync();


            // Загружаем всю сохранённую историю
            var history =
                await _messageRepository.GetAllAsync();

            foreach (var message in history)
            {
                var item = new ChatItem
                {
                    Id = message.Id,
                    Text = message.Text,
                    Status = message.Status,
                    IsMine = message.IsMine
                };

                _uiMessages[item.Id] = item;

                ChatList.Items.Add(item);
            }


            // Подключаем Bluetooth
            await _bluetoothTransport.ConnectAsync("");

            Debug.WriteLine(
                "Bluetooth connected"
            );


            _bluetoothChat = new ChatEngine(
                _bluetoothTransport,
                new CryptoService()
            );


            _bluetoothChat.MessageStatusChanged +=
                (id, status) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (_uiMessages.TryGetValue(
                                id,
                                out var item))
                        {
                            item.Status = "";
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


            // Получили сообщение от собеседника
            _bluetoothChat.MessageReceived +=
                async message =>
                {
                    var messageId =
                        Guid.NewGuid();

                    var item = new ChatItem
                    {
                        Id = messageId,
                        Text = message,
                        Status = "✓ Delivered",
                        IsMine = false
                    };


                    Debug.WriteLine(
                        $"ADDING INCOMING MESSAGE: " +
                        $"{item.Text}, IsMine={item.IsMine}"
                    );


                    // Сохраняем входящее сообщение в БД
                    await _messageRepository.AddAsync(
                        new StoredMessage
                        {
                            Id = messageId,
                            Text = message,
                            IsMine = false,
                            Timestamp = DateTime.Now,
                            Status = ""
                        });


                    // Добавляем сообщение в UI
                    Dispatcher.Invoke(() =>
                    {
                        _uiMessages[messageId] = item;

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
            Status = "",
            IsMine = true
        };


        // Сохраняем исходящее сообщение в БД
        await _messageRepository.AddAsync(
            new StoredMessage
            {
                Id = messageId,
                Text = text,
                IsMine = true,
                Timestamp = DateTime.Now,
                Status = ""
            });


        Debug.WriteLine(
            $"ADDING OWN MESSAGE: " +
            $"{item.Text}, IsMine={item.IsMine}"
        );


        _uiMessages[messageId] = item;


        ChatList.Items.Add(item);


        MessageInput.Clear();
    }
}

