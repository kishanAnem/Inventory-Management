using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> GetProductsByTenantAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Barcode == barcode && !p.IsDeleted);
    }
}
