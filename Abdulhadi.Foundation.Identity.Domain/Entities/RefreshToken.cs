namespace Abdulhadi.Foundation.Identity.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }

    public string Token { get; private set; } = default!;

    public Guid UserId { get; private set; }

    public ApplicationUser User { get; private set; } = default!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public string? ReplacedByToken { get; private set; }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked =>
        RevokedAt.HasValue;

    public bool IsActive =>
        !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int expiryDays)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),

            UserId = userId,

            Token = token,

            CreatedAt = DateTime.UtcNow,

            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTime.UtcNow;

        ReplacedByToken = replacedByToken;
    }
}