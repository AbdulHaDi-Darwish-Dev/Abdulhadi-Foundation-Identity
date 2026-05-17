using BuildingBlocks.Shared.Core;
using Microsoft.AspNetCore.Identity;
using Abdulhadi.Foundation.Identity.Domain.Common;

namespace Abdulhadi.Foundation.Identity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IEntity
{
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    private ApplicationUser() { }

    public static ApplicationUser Create(
        string email,
        string username,
        bool emailConfirmed)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username is required.", ErrorCodes.InvalidInput);

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.", ErrorCodes.InvalidInput);

        username = username.Trim().ToLower();
        email = email.Trim().ToLower();

        return new ApplicationUser
        {
            Id = Guid.NewGuid(),

            UserName = username,

            Email = email,
            EmailConfirmed = emailConfirmed,

            IsActive = true,
            IsDeleted = false,

            CreatedAt = DateTime.UtcNow
        };
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
    }
}