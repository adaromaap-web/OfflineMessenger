using Microsoft.EntityFrameworkCore;

namespace OfflineMessenger.Core.MessageHistory;

public class MessageRepository
{
    private readonly string _databasePath;

    public MessageRepository(string databasePath)
    {
        _databasePath = databasePath;
    }

    private MessageDbContext CreateContext()
    {
        return new MessageDbContext(_databasePath);
    }

    public async Task InitializeAsync()
    {
        await using var db = CreateContext();

        await db.Database.EnsureCreatedAsync();
    }

    public async Task<List<StoredMessage>> GetAllAsync()
    {
        await using var db = CreateContext();

        return await db.Messages
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task AddAsync(StoredMessage message)
    {
        await using var db = CreateContext();

        db.Messages.Add(message);

        await db.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await using var db = CreateContext();

        db.Messages.RemoveRange(db.Messages);

        await db.SaveChangesAsync();
    }
}