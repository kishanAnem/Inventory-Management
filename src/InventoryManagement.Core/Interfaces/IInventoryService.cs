using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IInventoryService
{
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task<bool> UpdateStockQuantityAsync(Guid productId, int quantity, string operation);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(Guid tenantId);
    Task<bool> IsProductAvailableAsync(Guid productId, int quantity);
    Task<string> GenerateBarcodeAsync(Product product);
}
