using Microsoft.EntityFrameworkCore;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.Email)
                   .IsUnique()
                   .HasFilter("[Email] IS NOT NULL");

        builder.HasIndex(u => u.UserName)
               .IsUnique();

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}