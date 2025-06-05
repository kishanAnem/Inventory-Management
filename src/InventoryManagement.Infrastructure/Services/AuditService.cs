using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantService _tenantService;

    public AuditService(
        IUnitOfWork unitOfWork,
        ITenantService tenantService)
    {
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
    }

    public async Task LogActivityAsync(
        string entityName,
        Guid entityId,
        string action,
        string userId,
        string? oldValues = null,
        string? newValues = null)
    {
        var tenantId = await _tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            throw new InvalidOperationException("Tenant context not found");
        }

        var auditLog = new AuditLog
        {
            TenantId = tenantId.Value,
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues ?? string.Empty,
            NewValues = newValues ?? string.Empty,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            IpAddress = string.Empty // TODO: Implement IP address capture
        };

        var repository = _unitOfWork.Repository<AuditLog>();
        await repository.AddAsync(auditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        var repository = (IAuditLogRepository)_unitOfWork.Repository<AuditLog>();
        return await repository.GetAuditLogsByTenantAsync(tenantId, startDate, endDate);
    }

    public async Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entityName, Guid entityId)
    {
        var repository = (IAuditLogRepository)_unitOfWork.Repository<AuditLog>();
        return await repository.GetEntityAuditLogsAsync(entityName, entityId);
    }

    public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId)
    {
        var repository = (IAuditLogRepository)_unitOfWork.Repository<AuditLog>();
        return await repository.GetUserAuditLogsAsync(userId);
    }

    public async Task<bool> PurgeOldLogsAsync(DateTime olderThan)
    {
        var repository = (IAuditLogRepository)_unitOfWork.Repository<AuditLog>();
        return await repository.PurgeOldLogsAsync(olderThan);
    }
}
