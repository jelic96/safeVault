using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeVault.Models;
using SafeVault.Repositories;
using SafeVault.Security;

namespace SafeVault.Controllers;

[ApiController]
[Route("api/vault")]
[Authorize(Policy = "VaultReaders")]
public sealed class VaultController(SecretRepository secrets, InputValidator validator) : ControllerBase
{
    [HttpGet("secrets")]
    public IActionResult List()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // API returns JSON. A browser UI must use OutputEncoder.ForHtml before rendering these values.
        return Ok(secrets.GetForOwner(userId));
    }

    [HttpPost("secrets")]
    public IActionResult Create(CreateSecretRequest request)
    {
        if (!validator.IsSafeLabel(request.Label)) return BadRequest(new { error = "Invalid label." });
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        secrets.Create(userId, request.Label, request.Value);
        return Created("/api/vault/secrets", new { message = "Secret stored." });
    }

    [HttpGet("admin/audit")]
    [Authorize(Policy = "VaultAdministrators")]
    public IActionResult Audit() => Ok(new { message = "Admin-only audit endpoint." });
}
