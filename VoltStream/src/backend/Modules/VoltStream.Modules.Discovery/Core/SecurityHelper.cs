namespace VoltStream.Modules.Discovery.Core;

using System.Security.Cryptography;
using System.Text;

public static class SecurityHelper
{
    public static string ComputeHmac(string message, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyHmac(string message, string signature, string secret)
        => ComputeHmac(message, secret) == signature;
}
