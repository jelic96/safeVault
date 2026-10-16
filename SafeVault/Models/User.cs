namespace SafeVault.Models;

public sealed record User(long Id, string Username, string PasswordHash, string Role);
