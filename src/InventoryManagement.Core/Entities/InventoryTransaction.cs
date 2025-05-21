using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Purchase, Sale, Adjustment
    public Guid ReferenceId { get; set; } // PurchaseId, SaleId, or AdjustmentId
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
