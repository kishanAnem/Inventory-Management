using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class PaymentTransaction : BaseEntity
{
    public Guid TenantId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Sale, Expenditure, Subscription
    public Guid ReferenceId { get; set; } // SaleId, ExpenditureId, or SubscriptionId
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? PaymentGatewayResponse { get; set; }
    public DateTime TransactionDate { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
}
