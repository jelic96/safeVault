using System.Text.Encodings.Web;

namespace SafeVault.Security;

public static class OutputEncoder
{
    // Use at every HTML rendering boundary; never store or return untrusted HTML as markup.
    public static string ForHtml(string untrustedValue) => HtmlEncoder.Default.Encode(untrustedValue);
}
