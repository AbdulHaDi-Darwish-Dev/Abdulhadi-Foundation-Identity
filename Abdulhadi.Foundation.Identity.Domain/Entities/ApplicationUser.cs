using Abdulhadi.Foundation.Identity.Domain.Common;
using BuildingBlocks.Shared.Core;
using Microsoft.AspNetCore.Identity;

namespace Abdulhadi.Foundation.Identity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IEntity
{
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    private ApplicationUser() { }

    public static ApplicationUser Create(
        string email,
        string username,
        string password,
        bool emailConfirmed)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username is required.", ErrorCodes.InvalidInput);

        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password is required.", ErrorCodes.InvalidInput);

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.", ErrorCodes.InvalidInput);

        username = username.Trim().ToLower();
        email = email.Trim().ToLower();

        return new ApplicationUser
        {
            Id = Guid.NewGuid(),

            UserName = username,
            Email = email,

            EmailConfirmed = false,

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