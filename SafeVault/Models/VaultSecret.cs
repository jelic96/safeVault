namespace SafeVault.Models;

public sealed record VaultSecret(long Id, long OwnerId, string Label, string ProtectedValue);
