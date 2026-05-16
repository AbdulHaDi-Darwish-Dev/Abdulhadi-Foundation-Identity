using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Auditing.Model;
using Microsoft.AspNetCore.Identity;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<AuditLog> AuditLogs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}