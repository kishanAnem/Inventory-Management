using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class Expenditure : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid ExpenditureTypeId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ExpenditureDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ExpenditureType ExpenditureType { get; set; } = null!;
}
