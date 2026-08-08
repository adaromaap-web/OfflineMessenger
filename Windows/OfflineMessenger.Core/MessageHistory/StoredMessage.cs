namespace OfflineMessenger.Core.MessageHistory;

public class StoredMessage
{
    public Guid Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsMine { get; set; }

    public DateTime Timestamp { get; set; }

    public string Status { get; set; } = string.Empty;
}