using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class Product : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal MRP { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsActive { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual CategoryType Category { get; set; } = null!;
}
