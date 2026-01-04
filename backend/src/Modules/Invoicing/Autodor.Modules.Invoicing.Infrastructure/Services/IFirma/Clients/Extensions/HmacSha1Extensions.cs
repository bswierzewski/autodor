using System.Security.Cryptography;
using System.Text;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;

public static class HmacSha1
{
    /// <summary>
    /// Computes HMAC-SHA1 hash of a message using a hexadecimal-encoded key.
    /// </summary>
    public static string Compute(string keyHex, string message)
    {
        ArgumentNullException.ThrowIfNull(keyHex);
        ArgumentNullException.ThrowIfNull(message);

        byte[] keyBytes = Convert.FromHexString(keyHex);
        using var hmac = new HMACSHA1(keyBytes);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
