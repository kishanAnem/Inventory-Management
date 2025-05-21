using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IAuditService
{
    Task LogActivityAsync(string entityName, Guid entityId, string action, string userId, string? oldValues = null, string? newValues = null);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entityName, Guid entityId);
    Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId);
    Task<bool> PurgeOldLogsAsync(DateTime olderThan);
}
