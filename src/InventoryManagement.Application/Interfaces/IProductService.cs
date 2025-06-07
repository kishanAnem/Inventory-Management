using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO> CreateAsync(CreateProductDTO createProductDto);
        Task<ProductDTO> UpdateAsync(UpdateProductDTO updateProductDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> GetStockQuantityAsync(Guid id);
        Task UpdateStockQuantityAsync(Guid id, int quantity);
    }
}
