using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.CurrentStock)
            .IsRequired();

        builder.Property(i => i.MinimumStock)
            .IsRequired();

        builder.Property(i => i.MaximumStock)
            .IsRequired();

        builder.Property(i => i.Location)
            .HasMaxLength(100);

        builder.Property(i => i.BatchNumber)
            .HasMaxLength(50);

        builder.HasOne(i => i.Tenant)
            .WithMany()
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.TenantId, i.ProductId })
            .IsUnique();
    }
}
