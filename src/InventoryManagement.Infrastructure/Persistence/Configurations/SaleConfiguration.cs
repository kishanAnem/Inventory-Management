using InventoryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SubTotal)
            .HasPrecision(18, 2);

        builder.Property(s => s.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(s => s.AmountPaid)
            .HasPrecision(18, 2);

        builder.Property(s => s.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.PaymentStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(s => s.Tenant)
            .WithMany()
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.TenantId, s.InvoiceNumber })
            .IsUnique();
    }
}
