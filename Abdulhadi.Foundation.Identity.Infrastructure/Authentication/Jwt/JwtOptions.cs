namespace  Abdulhadi.Foundation.Identity.Infrastructure.Authentication.Jwt;

public sealed class JwtOptions
{
    public string Issuer { get; init; }
    public string[] Audience { get; init; }
    public int ExpireMinutes { get; init; }
    public string SecretKey { get; init; }
}