using System.IO;

namespace OfflineMessenger.Core.MessageHistory;

public static class MessageDatabasePath
{
    public static string GetPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
            "OfflineMessenger");

        Directory.CreateDirectory(folder);

        return Path.Combine(
            folder,
            "messages.db");
    }
}