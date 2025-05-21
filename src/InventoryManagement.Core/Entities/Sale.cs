using System;
using System.Collections.Generic;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class Sale : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
