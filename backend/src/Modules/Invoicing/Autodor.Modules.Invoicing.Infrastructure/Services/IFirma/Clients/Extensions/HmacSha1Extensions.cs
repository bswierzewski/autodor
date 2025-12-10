using System.Security.Cryptography;
using System.Text;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;

/// <summary>
/// Utility class for HMAC-SHA1 cryptographic operations.
/// </summary>
public static class HmacSha1
{
    /// <summary>
    /// Computes HMAC-SHA1 hash of a message using a hexadecimal-encoded key.
    /// </summary>
    /// <param name="keyHex">The cryptographic key in hexadecimal format.</param>
    /// <param name="message">The message to be hashed.</param>
    /// <returns>The HMAC-SHA1 hash in lowercase hexadecimal format.</returns>
    /// <exception cref="ArgumentNullException">Thrown when keyHex or message is null.</exception>
    /// <exception cref="FormatException">Thrown when keyHex is not a valid hexadecimal string.</exception>
    public static string Compute(string keyHex, string message)
    {
        if (keyHex == null)
            throw new ArgumentNullException(nameof(keyHex));

        if (message == null)
            throw new ArgumentNullException(nameof(message));

        byte[] keyBytes = Convert.FromHexString(keyHex);
        using var hmac = new HMACSHA1(keyBytes);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
