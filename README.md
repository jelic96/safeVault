# SafeVault — Secure Vault API

SafeVault is an ASP.NET Core 8 application built as a security-focused course submission. It demonstrates secure input handling, authentication, role-based authorization, SQL injection prevention, XSS prevention, and automated security tests.

## Rubric coverage

| Requirement | Where it is demonstrated |
| --- | --- |
| Input validation and SQL injection prevention | `InputValidator.cs`; SQLite commands with named parameters in both repositories |
| Authentication | Registration and cookie-based sign-in in `AuthController` and `AuthService` |
| Authorization / RBAC | `User` and `Admin` roles; `VaultReaders` and `VaultAdministrators` policies |
| SQL injection and XSS fixes | Parameterized SQL, strict validation, output encoding helper, security headers / CSP |
| Security tests | `SafeVault.Tests/SecurityTests.cs` |
| Vulnerability summary | `SECURITY_SUMMARY.md` |

## Run in Visual Studio

1. Open `SafeVault.sln` in Visual Studio 2022.
2. Let Visual Studio restore the NuGet packages.
3. Select **SafeVault** as the startup project and press `Ctrl+F5` (or `F5`). The development profile uses HTTPS.
4. Run tests from **Test > Test Explorer > Run All**.

You can also run `dotnet test` from the solution folder.

## API walkthrough

Create an account first. Usernames allow only letters, digits, dot, underscore, and hyphen. Passwords must be at least 12 characters and include uppercase, lowercase, a digit, and a symbol.

```http
POST /api/auth/register
Content-Type: application/json

{ "username": "student", "password": "StrongPassword1!" }
```

Then use `/api/auth/login` with the same credentials. A successful sign-in creates an HTTP-only, Secure, SameSite=Strict cookie. Authenticated users can call `GET` and `POST /api/vault/secrets`; the `/api/vault/admin/audit` endpoint is reserved for the `Admin` role.

## GitHub submission

Create a **public** GitHub repository called `safevault-security-project`, then push this whole folder. Do not upload `safevault.db`, `.vs`, `bin`, or `obj`; `.gitignore` already excludes them. Copy the repository URL into Coursera.

## Copilot evidence — complete honestly before submitting

The course asks specifically for Microsoft Copilot usage. This project includes the final secure implementation, but a student must provide truthful evidence of their own Copilot interaction. In Visual Studio, use Copilot Chat to ask it to explain or improve the following, then retain screenshots/history if your reviewer requests it:

1. “Review `UserRepository.cs` for SQL injection risks and explain why parameterized queries are safe.”
2. “Review `AuthController.cs` and explain the authentication cookie security settings and RBAC policies.”
3. “Create additional tests for XSS output encoding and injection attempts in this project.”

Do not claim Copilot generated work it did not generate. You can say it was used to review and explain the final secure code only if you actually did that.
