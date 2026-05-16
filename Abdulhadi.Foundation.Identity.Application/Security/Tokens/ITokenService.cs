namespace Abdulhadi.Foundation.Identity.Application.Security.Tokens;

public interface ITokenService
{
    string GenerateToken(int size = 32);
    string HashToken(string token);
}