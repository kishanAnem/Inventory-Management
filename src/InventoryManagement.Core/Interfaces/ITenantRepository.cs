using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ITenantRepository : IGenericRepository<Tenant>
{
    Task<Tenant?> GetByNameAsync(string name);
    Task<bool> IsSubscriptionValidAsync(Guid tenantId);
}
