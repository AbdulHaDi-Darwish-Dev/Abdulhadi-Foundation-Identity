using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public static readonly Guid OwnerRoleId = Guid.Parse("7c1a89c3-1234-4321-ba98-765432109876");
    public static readonly Guid AdminRoleId = Guid.Parse("7c1a89c3-1234-4321-ba98-111122223333");
    public static readonly Guid UserRoleId = Guid.Parse("1a2b3c4d-5678-8765-4321-abcdef012345");

    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = OwnerRoleId,
                Name = "Owner",
                NormalizedName = "OWNER"
            },
            new IdentityRole<Guid>
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
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