using Microsoft.Extensions.Configuration;
using SafeVault.Data;
using SafeVault.Repositories;
using SafeVault.Security;
using Xunit;

namespace SafeVault.Tests;

public sealed class SecurityTests
{
    private readonly InputValidator _validator = new();

    [Theory]
    [InlineData("admin' OR '1'='1")]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("two words")]
    public void Username_validation_rejects_malicious_or_invalid_input(string input)
        => Assert.False(_validator.IsValidUsername(input));

    [Fact]
    public void Password_policy_requires_length_and_character_variety()
    {
        Assert.True(_validator.IsStrongPassword("StrongPassword1!"));
        Assert.False(_validator.IsStrongPassword("short1!A"));
        Assert.False(_validator.IsStrongPassword("alllowercase123!"));
    }

    [Fact]
    public void Html_output_is_encoded_to_prevent_xss()
    {
        var encoded = OutputEncoder.ForHtml("<img src=x onerror=alert('xss')>");
        Assert.DoesNotContain("<img", encoded, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("&lt;img", encoded);
    }

    [Fact]
    public void Secret_labels_reject_html_script_markup()
        => Assert.False(_validator.IsSafeLabel("<script>alert(1)</script>"));

    [Fact]
    public void Parameterized_lookup_does_not_treat_injection_as_sql()
    {
        var path = Path.Combine(Path.GetTempPath(), $"safevault-test-{Guid.NewGuid():N}.db");
        try
        {
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?> { ["ConnectionStrings:SafeVault"] = $"Data Source={path}" }).Build();
            var database = new SqliteConnectionFactory(config);
            database.Initialize();
            var users = new UserRepository(database);
            Assert.True(users.Create("alice", "not-a-real-hash"));

            Assert.Null(users.FindByUsername("alice' OR '1'='1"));
            Assert.NotNull(users.FindByUsername("alice"));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
