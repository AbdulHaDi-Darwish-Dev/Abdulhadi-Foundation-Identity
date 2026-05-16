using System.Text;
using System.Security.Cryptography;

namespace Abdulhadi.Foundation.Identity.Application.Security.Tokens;

public class TokenService : ITokenService
{
    public string GenerateToken(int size = 32)
    {
        var bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
    }

    public string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(bytes);
    }
}