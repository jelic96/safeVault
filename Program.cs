using Microsoft.AspNetCore.Authentication.Cookies;
using SafeVault.Data;
using SafeVault.Repositories;
using SafeVault.Security;
using SafeVault.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDataProtection();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VaultReaders", policy => policy.RequireRole("User", "Admin"));
    options.AddPolicy("VaultAdministrators", policy => policy.RequireRole("Admin"));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.SlidingExpiration = true;
        options.LoginPath = "/api/auth/login";
        options.AccessDeniedPath = "/api/auth/denied";
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });
builder.Services.AddSingleton<InputValidator>();
builder.Services.AddSingleton<SqliteConnectionFactory>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SecretRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SecretProtector>();

var app = builder.Build();

app.UseExceptionHandler("/api/error");
app.UseHsts();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none'; base-uri 'self'");
    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Content("<h1>SafeVault</h1><p>Secure vault API is running.</p>", "text/html"));

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<SqliteConnectionFactory>();
    database.Initialize();
}

app.Run();

public partial class Program { }
