using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<AuditLog>> GetAuditLogsByTenantAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(a => 
                a.TenantId == tenantId && 
                a.Timestamp >= startDate && 
                a.Timestamp <= endDate)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AuditLog>> GetEntityAuditLogsAsync(string entityName, Guid entityId)
    {
        return await _dbSet
            .Where(a => 
                a.EntityName == entityName && 
                a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AuditLog>> GetUserAuditLogsAsync(string userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<bool> PurgeOldLogsAsync(DateTime olderThan)
    {
        var oldLogs = await _dbSet
            .Where(a => a.Timestamp < olderThan)
            .ToListAsync();

        _dbSet.RemoveRange(oldLogs);
        await _context.SaveChangesAsync();

        return true;
    }
}
