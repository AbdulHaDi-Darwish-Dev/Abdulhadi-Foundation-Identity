using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public static readonly Guid SuperAdminRoleId = Guid.Parse("7c1a89c3-1234-4321-ba98-765432109876");
    public static readonly Guid UserRoleId = Guid.Parse("1a2b3c4d-5678-8765-4321-abcdef012345");

    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = SuperAdminRoleId,
                Name = "Super Admin",
                NormalizedName = "SUPER ADMIN"
            },
            new IdentityRole<Guid>
            {
                Id = UserRoleId,
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}