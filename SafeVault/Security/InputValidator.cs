using System.Text.RegularExpressions;

namespace SafeVault.Security;

public sealed class InputValidator
{
    private static readonly Regex Username = new("^[a-zA-Z0-9_.-]{3,32}$", RegexOptions.Compiled);

    public bool IsValidUsername(string? value) => value is not null && Username.IsMatch(value);

    public bool IsStrongPassword(string? value) => value is not null && value.Length >= 12
        && value.Any(char.IsUpper) && value.Any(char.IsLower) && value.Any(char.IsDigit)
        && value.Any(ch => !char.IsLetterOrDigit(ch));

    public bool IsSafeLabel(string? value) => value is { Length: > 0 and <= 80 }
        && !value.Any(char.IsControl)
        && !value.Contains('<')
        && !value.Contains('>');
}
