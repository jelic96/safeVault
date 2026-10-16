using Microsoft.AspNetCore.Identity;
using SafeVault.Models;
using SafeVault.Repositories;
using SafeVault.Security;

namespace SafeVault.Services;

public sealed class AuthService(UserRepository users, InputValidator validator)
{
    private readonly PasswordHasher<User> _hasher = new();

    public bool Register(RegisterRequest request)
    {
        if (!validator.IsValidUsername(request.Username) || !validator.IsStrongPassword(request.Password)) return false;
        var temporaryUser = new User(0, request.Username, string.Empty, "User");
        return users.Create(request.Username, _hasher.HashPassword(temporaryUser, request.Password));
    }

    public User? Authenticate(LoginRequest request)
    {
        if (!validator.IsValidUsername(request.Username) || string.IsNullOrEmpty(request.Password)) return null;
        var user = users.FindByUsername(request.Username);
        if (user is null) return null;
        return _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded
            ? user : null;
    }
}
