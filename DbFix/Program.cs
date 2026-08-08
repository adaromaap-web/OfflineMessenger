using Microsoft.Data.Sqlite;

var dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "OfflineMessenger",
    "messages.db"
);

Console.WriteLine($"Database: {dbPath}");

if (!File.Exists(dbPath))
{
    Console.WriteLine("Database not found.");
    return;
}

var connectionString = $"Data Source={dbPath}";

using var connection = new SqliteConnection(connectionString);
connection.Open();

using var command = connection.CreateCommand();

command.CommandText = """
    CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
        "MigrationId" TEXT NOT NULL
            CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
        "ProductVersion" TEXT NOT NULL
    );

    INSERT OR IGNORE INTO "__EFMigrationsHistory"
        ("MigrationId", "ProductVersion")
    VALUES
        ('20260808022731_InitialMessageHistory', '8.0.19');
    """;

command.ExecuteNonQuery();

Console.WriteLine("EF migration history synchronized.");

command.CommandText = """
    SELECT COUNT(*) FROM "Messages";
    """;

var messageCount = command.ExecuteScalar();

Console.WriteLine($"Messages in database: {messageCount}");