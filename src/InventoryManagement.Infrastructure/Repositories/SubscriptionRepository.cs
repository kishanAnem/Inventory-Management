using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Subscription?> GetActiveTenantSubscriptionAsync(Guid tenantId)
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .FirstOrDefaultAsync(s => 
                s.TenantId == tenantId && 
                !s.IsDeleted &&
                s.StartDate <= currentDate && 
                s.EndDate >= currentDate &&
                s.Status == "Active");
    }

    public async Task<IReadOnlyList<Subscription>> GetSubscriptionHistoryAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(s => s.TenantId == tenantId && !s.IsDeleted)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();
    }

    public async Task<bool> HasActiveSubscriptionAsync(Guid tenantId)
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .AnyAsync(s => 
                s.TenantId == tenantId && 
                !s.IsDeleted &&
                s.StartDate <= currentDate && 
                s.EndDate >= currentDate &&
                s.Status == "Active");
    }
}
