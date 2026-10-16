using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models;

public sealed class RegisterRequest
{
    [Required, StringLength(32, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 12)]
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, StringLength(32, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 12)]
    public string Password { get; init; } = string.Empty;
}

public sealed class CreateSecretRequest
{
    [Required, StringLength(80, MinimumLength = 1)]
    public string Label { get; init; } = string.Empty;

    [Required, StringLength(500, MinimumLength = 1)]
    public string Value { get; init; } = string.Empty;
}
