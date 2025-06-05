using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId && !i.IsDeleted);
    }

    public async Task<IReadOnlyList<Inventory>> GetLowStockItemsAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(i => i.Product)
            .Where(i => 
                i.TenantId == tenantId && 
                !i.IsDeleted && 
                i.Quantity <= i.MinimumStock)
            .ToListAsync();
    }

    public async Task UpdateStockAsync(Guid productId, int quantity, string operation)
    {
        var inventory = await GetByProductIdAsync(productId);
        if (inventory == null)
            throw new InvalidOperationException($"No inventory found for product {productId}");

        switch (operation.ToLower())
        {
            case "add":
                inventory.Quantity += quantity;
                break;
            case "remove":
                if (inventory.Quantity < quantity)
                    throw new InvalidOperationException("Insufficient stock");
                inventory.Quantity -= quantity;
                break;
            default:
                throw new InvalidOperationException($"Invalid stock operation: {operation}");
        }

        await UpdateAsync(inventory);
    }
}
