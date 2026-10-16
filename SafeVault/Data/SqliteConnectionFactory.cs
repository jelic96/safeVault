using Microsoft.Data.Sqlite;

namespace SafeVault.Data;

public sealed class SqliteConnectionFactory(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("SafeVault")
        ?? throw new InvalidOperationException("Connection string SafeVault is missing.");

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Initialize()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE COLLATE NOCASE,
                PasswordHash TEXT NOT NULL,
                Role TEXT NOT NULL CHECK (Role IN ('User', 'Admin'))
            );
            CREATE TABLE IF NOT EXISTS Secrets (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OwnerId INTEGER NOT NULL,
                Label TEXT NOT NULL,
                ProtectedValue TEXT NOT NULL,
                FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
