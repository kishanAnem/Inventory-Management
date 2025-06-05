using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IUnitOfWork unitOfWork,
        IInventoryService inventoryService,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _productRepository = (IProductRepository)unitOfWork.Repository<Product>();
        _inventoryService = inventoryService;
        _auditService = auditService;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        // Generate barcode for the product
        product.Barcode = await _inventoryService.GenerateBarcodeAsync(product);
        
        var addedProduct = await _productRepository.AddAsync(product);
        
        await _auditService.LogActivityAsync(
            "Product",
            addedProduct.Id,
            "Create",
            product.CreatedBy);

        return addedProduct;
    }

    public async Task<Product?> GetProductByBarcodeAsync(string barcode)
    {
        return await _productRepository.GetByBarcodeAsync(barcode);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
    {
        return await _productRepository.GetProductsByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<Product>> GetProductsByTenantAsync(Guid tenantId)
    {
        return await _productRepository.GetProductsByTenantAsync(tenantId);
    }

    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = await _productRepository.GetByIdAsync(product.Id)
            ?? throw new InvalidOperationException($"Product with ID {product.Id} not found.");

        var oldValues = System.Text.Json.JsonSerializer.Serialize(existingProduct);
        
        await _productRepository.UpdateAsync(product);
        
        await _auditService.LogActivityAsync(
            "Product",
            product.Id,
            "Update",
            product.LastModifiedBy ?? "system",
            oldValues,
            System.Text.Json.JsonSerializer.Serialize(product));
    }

    public async Task DeleteProductAsync(Guid productId, string userId)
    {
        var product = await _productRepository.GetByIdAsync(productId)
            ?? throw new InvalidOperationException($"Product with ID {productId} not found.");

        // Soft delete
        product.IsDeleted = true;
        product.DeletedBy = userId;
        product.DeletedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        
        await _auditService.LogActivityAsync(
            "Product",
            productId,
            "Delete",
            userId);
    }
}
