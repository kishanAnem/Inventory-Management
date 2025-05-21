using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ITenantService
{
    Task<Tenant> CreateTenantAsync(Tenant tenant);
    Task<bool> ActivateTenantAsync(Guid tenantId);
    Task<bool> DeactivateTenantAsync(Guid tenantId);
    Task<bool> UpdateTenantSettingsAsync(Tenant tenant);
    Task<Tenant?> GetTenantByIdAsync(Guid tenantId);
    Task<bool> IsTenantActiveAsync(Guid tenantId);
    Task<IEnumerable<Tenant>> GetAllActiveTenants();
    Guid? GetCurrentTenantId(); 
}
