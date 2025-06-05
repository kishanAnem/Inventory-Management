using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class CategoryTypeRepository : GenericRepository<CategoryType>, ICategoryTypeRepository
{
    public CategoryTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<CategoryType>> GetActiveCategoriesAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(c => 
                c.TenantId == tenantId && 
                !c.IsDeleted && 
                c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<CategoryType?> GetByNameAsync(string name, Guid tenantId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => 
                c.TenantId == tenantId && 
                c.Name.ToLower() == name.ToLower() && 
                !c.IsDeleted);
    }

    public async Task<bool> IsCategoryUsedAsync(Guid categoryId)
    {
        return await _context.Set<Product>()
            .AnyAsync(p => p.CategoryId == categoryId && !p.IsDeleted);
    }
}
