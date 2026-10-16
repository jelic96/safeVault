using SafeVault.Data;
using SafeVault.Models;
using SafeVault.Services;

namespace SafeVault.Repositories;

public sealed class SecretRepository(SqliteConnectionFactory database, SecretProtector protector)
{
    public IReadOnlyList<VaultSecret> GetForOwner(long ownerId)
    {
        var secrets = new List<VaultSecret>();
        using var connection = database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, OwnerId, Label, ProtectedValue FROM Secrets WHERE OwnerId = $ownerId";
        command.Parameters.AddWithValue("$ownerId", ownerId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
            secrets.Add(new VaultSecret(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2), reader.GetString(3)) with { ProtectedValue = protector.Unprotect(reader.GetString(3)) });
        return secrets;
    }

    public void Create(long ownerId, string label, string protectedValue)
    {
        using var connection = database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Secrets (OwnerId, Label, ProtectedValue) VALUES ($ownerId, $label, $value)";
        command.Parameters.AddWithValue("$ownerId", ownerId);
        command.Parameters.AddWithValue("$label", label);
        command.Parameters.AddWithValue("$value", protector.Protect(protectedValue));
        command.ExecuteNonQuery();
    }
}
