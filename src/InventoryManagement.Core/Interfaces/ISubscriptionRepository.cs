using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ISubscriptionRepository : IGenericRepository<Subscription>
{
    Task<Subscription?> GetActiveTenantSubscriptionAsync(Guid tenantId);
    Task<IReadOnlyList<Subscription>> GetSubscriptionHistoryAsync(Guid tenantId);
    Task<bool> HasActiveSubscriptionAsync(Guid tenantId);
}
