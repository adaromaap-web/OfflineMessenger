using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OfflineMessenger.UI.Windows;

public class ChatItem : INotifyPropertyChanged
{
    public Guid Id { get; set; }


    public string Text { get; set; } = "";


    private string _status = "";

    public string Status
    {
        get => _status;

        set
        {
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayText));
        }
    }


    public string DisplayText =>
        $"You: {Text}   {Status}";



    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}