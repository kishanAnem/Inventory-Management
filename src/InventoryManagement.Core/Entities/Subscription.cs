using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class Subscription : BaseEntity
{
    public Guid TenantId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool AutoRenew { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? LastBillingDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public string? PaymentGatewaySubscriptionId { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
}
