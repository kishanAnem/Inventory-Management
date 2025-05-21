using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private Guid? _currentTenantId;

    public TenantService(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Tenant> CreateTenantAsync(Tenant tenant)
    {
        // Set default values for new tenant
        tenant.IsActive = true;
        tenant.SubscriptionStartDate = DateTime.UtcNow;
        tenant.SubscriptionEndDate = DateTime.UtcNow.AddYears(1); // Default 1 year subscription

        return await _tenantRepository.AddAsync(tenant);
    }

    public async Task<bool> ActivateTenantAsync(Guid tenantId)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) return false;

        tenant.IsActive = true;
        await _tenantRepository.UpdateAsync(tenant);
        return true;
    }

    public async Task<bool> DeactivateTenantAsync(Guid tenantId)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) return false;

        tenant.IsActive = false;
        await _tenantRepository.UpdateAsync(tenant);
        return true;
    }

    public async Task<bool> UpdateTenantSettingsAsync(Tenant tenant)
    {
        var existingTenant = await _tenantRepository.GetByIdAsync(tenant.Id);
        if (existingTenant == null) return false;

        // Preserve subscription and status info
        tenant.IsActive = existingTenant.IsActive;
        tenant.SubscriptionStartDate = existingTenant.SubscriptionStartDate;
        tenant.SubscriptionEndDate = existingTenant.SubscriptionEndDate;

        await _tenantRepository.UpdateAsync(tenant);
        return true;
    }

    public async Task<Tenant?> GetTenantByIdAsync(Guid tenantId)
    {
        return await _tenantRepository.GetByIdAsync(tenantId);
    }

    public async Task<bool> IsTenantActiveAsync(Guid tenantId)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant == null) return false;
        
        return tenant.IsActive && tenant.SubscriptionEndDate > DateTime.UtcNow;
    }

    public async Task<IEnumerable<Tenant>> GetAllActiveTenants()
    {
        var allTenants = await _tenantRepository.GetAllAsync();
        return allTenants.Where(t => t.IsActive && t.SubscriptionEndDate > DateTime.UtcNow);
    }

    // Existing methods for current tenant management
    public Guid? GetCurrentTenantId()
    {
        return _currentTenantId;
    }

    public Task<bool> SetCurrentTenantAsync(string tenantId)
    {
        if (Guid.TryParse(tenantId, out Guid parsedTenantId))
        {
            _currentTenantId = parsedTenantId;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ValidateTenantAsync(string tenantId)
    {
        // TODO: Implement tenant validation logic
        return Task.FromResult(true);
    }

    public Task ClearCurrentTenantAsync()
    {
        _currentTenantId = null;
        return Task.CompletedTask;
    }
}
