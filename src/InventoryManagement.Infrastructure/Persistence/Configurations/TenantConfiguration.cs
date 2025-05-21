using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.LogoUrl)
            .HasMaxLength(500);

        builder.Property(t => t.Address)
            .HasMaxLength(200);

        builder.Property(t => t.City)
            .HasMaxLength(100);

        builder.Property(t => t.State)
            .HasMaxLength(100);

        builder.Property(t => t.Mobile)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Email)
            .IsUnique();

        builder.HasIndex(t => t.Mobile)
            .IsUnique();
    }
}
