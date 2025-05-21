using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<Product>> GetProductsByTenantAsync(Guid tenantId);
    Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
    Task<Product?> GetByBarcodeAsync(string barcode);
}
