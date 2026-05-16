using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Auditing.Model;
using BuildingBlocks.Auditing.Attributes;
using BuildingBlocks.Logging.Abstractions;
using BuildingBlocks.Auditing.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Auditing.Interceptors;

public class AuditableInterceptor : SaveChangesInterceptor
{
    private readonly IRequestContext _requestContext;

    public AuditableInterceptor(IRequestContext requestContext)
    {
        _requestContext = requestContext;
    }

    private static readonly HashSet<string> SensitiveProperties = new()
    {
        "PasswordHash",
        "SecurityStamp",
        "ConcurrencyStamp",
        "NormalizedUserName",
        "NormalizedEmail",
        "TwoFactorEnabled",
        "PhoneNumber",
        "PhoneNumberConfirmed",
        "EmailConfirmed"
    };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        OnBeforeSaveChanges(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void OnBeforeSaveChanges(DbContext? context)
    {
        if (context == null) return;

        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // مراقبة الكيانات التي تنفذ الواجهة IAuditableEntity فقط
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            if (entry.Entity is not IAuditableEntity)
                continue;

            var auditEntry = new AuditLog
            {
                UserId = _requestContext.UserId,
                IpAddress = _requestContext.IpAddress,
                TableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                Action = entry.State.ToString()
            };

            var keyValues = new Dictionary<string, object?>();
            foreach (var prop in entry.Metadata.FindPrimaryKey()!.Properties)
            {
                keyValues[prop.Name] = entry.Property(prop.Name).CurrentValue;
            }
            auditEntry.KeyValues = JsonSerializer.Serialize(keyValues);

            var oldValues = new Dictionary<string, object?>();
            var newValues = new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue;

                var propertyName = property.Metadata.Name;
                var entityType = entry.Entity.GetType();
                var clrProperty = entityType.GetProperty(propertyName);

                // فحص الحساسية باستخدام السمة [SensitiveData] and Blacklist

                var isAttributeSensitive = clrProperty != null &&
                    Attribute.IsDefined(clrProperty, typeof(SensitiveDataAttribute), true);

                var isBlacklisted = SensitiveProperties.Contains(propertyName);

                bool isSensitive = isAttributeSensitive || isBlacklisted;

                object? oldVal = property.OriginalValue;
                object? newVal = property.CurrentValue;

                if (isSensitive)
                {
                    oldVal = "*** MASKED ***";
                    newVal = "*** MASKED ***";
                }

                if (entry.State == EntityState.Added)
                {
                    newValues[propertyName] = newVal;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    oldValues[propertyName] = oldVal;
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (property.IsModified)
                    {
                        oldValues[propertyName] = oldVal;
                        newValues[propertyName] = newVal;
                    }
                }
            }

            auditEntry.OldValues = oldValues.Count > 0 ? JsonSerializer.Serialize(oldValues) : null;
            auditEntry.NewValues = newValues.Count > 0 ? JsonSerializer.Serialize(newValues) : null;

            auditEntries.Add(auditEntry);
        }

        // إضافة سجلات التدقيق إلى الـ DbContext لتُحفظ تلقائياً في نفس العملية
        if (auditEntries.Any())
        {
            context.AddRange(auditEntries);
        }
    }
}