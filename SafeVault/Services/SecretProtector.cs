using Microsoft.AspNetCore.DataProtection;

namespace SafeVault.Services;

public sealed class SecretProtector(IDataProtectionProvider dataProtection)
{
    private readonly IDataProtector _protector = dataProtection.CreateProtector("SafeVault.Secret.v1");

    public string Protect(string plaintext) => _protector.Protect(plaintext);
    public string Unprotect(string ciphertext) => _protector.Unprotect(ciphertext);
}
