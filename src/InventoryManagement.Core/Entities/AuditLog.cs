using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class AuditLog : BaseEntity
{
    public Guid TenantId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, etc.
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
}
