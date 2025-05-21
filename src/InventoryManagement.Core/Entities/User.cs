using System;
using InventoryManagement.Core.Entities.Common;

namespace InventoryManagement.Core.Entities;

public class User : BaseEntity
{
    public Guid TenantId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public virtual Tenant Tenant { get; set; } = null!;
}
