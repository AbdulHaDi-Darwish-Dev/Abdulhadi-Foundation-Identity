namespace Abdulhadi.Foundation.Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public Guid UserId { get; set; }
}