using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class ExpenditureTypeRepository : GenericRepository<ExpenditureType>, IExpenditureTypeRepository
{
    public ExpenditureTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ExpenditureType>> GetActiveTypesAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(e => 
                e.TenantId == tenantId && 
                !e.IsDeleted && 
                e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<ExpenditureType?> GetByNameAsync(string name, Guid tenantId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => 
                e.TenantId == tenantId && 
                e.Name.ToLower() == name.ToLower() && 
                !e.IsDeleted);
    }

    public async Task<bool> IsTypeUsedAsync(Guid typeId)
    {
        return await _context.Set<Expenditure>()
            .AnyAsync(e => e.ExpenditureTypeId == typeId && !e.IsDeleted);
    }
}
