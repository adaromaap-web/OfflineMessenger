using Microsoft.EntityFrameworkCore;

namespace OfflineMessenger.Core.MessageHistory;

public class MessageDbContext : DbContext
{
    public DbSet<StoredMessage> Messages => Set<StoredMessage>();

    private readonly string _databasePath;

    public MessageDbContext(string databasePath)
    {
        _databasePath = databasePath;
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            $"Data Source={_databasePath}");
    }
}