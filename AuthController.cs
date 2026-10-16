using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SafeVault.Models;
using SafeVault.Services;

namespace SafeVault.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request) => auth.Register(request)
        ? Created("/api/auth/login", new { message = "Account created." })
        : BadRequest(new { error = "Username or password does not meet the security policy, or the username is taken." });

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = auth.Authenticate(request);
        if (user is null) return Unauthorized(new { error = "Invalid credentials." });
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role) };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return Ok(new { message = "Signed in.", role = user.Role });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Signed out." });
    }

    [HttpGet("denied")]
    public IActionResult Denied() => StatusCode(StatusCodes.Status403Forbidden);
}
