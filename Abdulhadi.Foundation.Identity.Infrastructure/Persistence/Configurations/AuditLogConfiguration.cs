using BuildingBlocks.Auditing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // 1. اسم الجدول
        builder.ToTable("AuditLogs");

        // 2. المفتاح الأساسي
        builder.HasKey(x => x.Id);

        // 3. الحقول النصية الأساسية
        builder.Property(x => x.UserId)
            .HasMaxLength(450); // طول مناسب لمعرفات المستخدمين سواء كانت String أو GUID

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45); // يدعم IPv4 و IPv6

        builder.Property(x => x.TableName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(50);

        // 4. الحقول النصية الكبيرة للبيانات (JSON)
        builder.Property(x => x.KeyValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValues)
            .HasColumnType("nvarchar(max)");

        // 5. التاريخ والوقت
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}