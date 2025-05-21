using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    Task<Inventory?> GetByProductIdAsync(Guid productId);
    Task<IReadOnlyList<Inventory>> GetLowStockItemsAsync(Guid tenantId);
    Task UpdateStockAsync(Guid productId, int quantity, string operation);
}
