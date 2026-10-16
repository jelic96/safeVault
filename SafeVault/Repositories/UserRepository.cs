using Microsoft.Data.Sqlite;
using SafeVault.Data;
using SafeVault.Models;

namespace SafeVault.Repositories;

public sealed class UserRepository(SqliteConnectionFactory database)
{
    public User? FindByUsername(string username)
    {
        using var connection = database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = $username";
        command.Parameters.AddWithValue("$username", username); // Parameter binding prevents SQL injection.
        using var reader = command.ExecuteReader();
        return reader.Read() ? new User(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)) : null;
    }

    public bool Create(string username, string passwordHash, string role = "User")
    {
        using var connection = database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (Username, PasswordHash, Role) VALUES ($username, $passwordHash, $role)";
        command.Parameters.AddWithValue("$username", username);
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        command.Parameters.AddWithValue("$role", role);
        try { return command.ExecuteNonQuery() == 1; }
        catch (SqliteException) { return false; }
    }
}
