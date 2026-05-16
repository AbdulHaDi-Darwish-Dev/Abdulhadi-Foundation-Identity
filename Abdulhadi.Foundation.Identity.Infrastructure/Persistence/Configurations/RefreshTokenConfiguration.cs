using Microsoft.EntityFrameworkCore;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace  Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasMaxLength(36).IsFixedLength();

        builder.Property(r => r.UserId).HasMaxLength(36).IsFixedLength();

        builder.Property(t => t.Token).HasMaxLength(100);

        builder.Property(t => t.IsRevoke).HasDefaultValue(false);

        builder.Property(t => t.RevokedAt);

        builder.Property(t => t.ExpiresAt);
    }
}