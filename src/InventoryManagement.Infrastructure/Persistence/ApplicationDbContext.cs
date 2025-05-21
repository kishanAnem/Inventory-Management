using System;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Entities.Common;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext
{
    private readonly Guid? _tenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantId = tenantService.GetCurrentTenantId();
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CategoryType> Categories => Set<CategoryType>();
    public DbSet<ExpenditureType> ExpenditureTypes => Set<ExpenditureType>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Expenditure> Expenditures => Set<Expenditure>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Configure global query filters for multi-tenancy
        builder.Entity<Product>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<Inventory>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<Customer>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<CategoryType>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<ExpenditureType>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<Sale>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<Expenditure>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<PaymentTransaction>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<Subscription>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
        builder.Entity<AuditLog>().HasQueryFilter(e => !_tenantId.HasValue || e.TenantId == _tenantId.Value);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set audit fields before saving
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _tenantId?.ToString() ?? "system"; // TODO: Replace with actual user ID
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = _tenantId?.ToString() ?? "system"; // TODO: Replace with actual user ID
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = _tenantId?.ToString() ?? "system"; // TODO: Replace with actual user ID
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
