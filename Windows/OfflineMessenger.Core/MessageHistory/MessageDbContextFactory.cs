using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OfflineMessenger.Core.MessageHistory;

public class MessageDbContextFactory
    : IDesignTimeDbContextFactory<MessageDbContext>
{
    public MessageDbContext CreateDbContext(string[] args)
    {
        var appData =
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);

        var folder =
            Path.Combine(
                appData,
                "OfflineMessenger");

        Directory.CreateDirectory(folder);

        var databasePath =
            Path.Combine(
                folder,
                "messages.db");

        return new MessageDbContext(databasePath);
    }
}