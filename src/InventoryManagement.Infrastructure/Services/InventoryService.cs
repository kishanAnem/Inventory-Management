using System.Text.RegularExpressions;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _inventoryRepository = (IInventoryRepository)unitOfWork.Repository<Inventory>();
        _productRepository = (IProductRepository)unitOfWork.Repository<Product>();
        _auditService = auditService;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        // First add the product
        await _productRepository.AddAsync(product);

        // Create initial inventory record
        var inventory = new Inventory
        {
            ProductId = product.Id,
            TenantId = product.TenantId,
            Quantity = 0,
            PurchaseDate = DateTime.UtcNow,
            CreatedBy = product.CreatedBy
        };

        await _inventoryRepository.AddAsync(inventory);

        await _auditService.LogActivityAsync(
            "Product",
            product.Id,
            "Create",
            product.CreatedBy);

        return product;
    }

    public async Task<string> GenerateBarcodeAsync(Product product)
    {
        // Generate a simple barcode based on tenant and product details
        // In a real application, you might want to use a proper barcode generation library
        var timestamp = DateTime.UtcNow.Ticks.ToString()[^6..];
        var tenantPart = product.TenantId.ToString()[^4..];
        var barcode = $"{tenantPart}{timestamp}";

        // Ensure uniqueness
        while (await _productRepository.GetByBarcodeAsync(barcode) != null)
        {
            timestamp = DateTime.UtcNow.Ticks.ToString()[^6..];
            barcode = $"{tenantPart}{timestamp}";
            await Task.Delay(1); // Small delay to ensure different timestamp
        }

        return barcode;
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(Guid tenantId)
    {
        var lowStockInventories = await _inventoryRepository.GetLowStockItemsAsync(tenantId);
        var products = new List<Product>();

        foreach (var inventory in lowStockInventories)
        {
            if (inventory.Product != null)
            {
                products.Add(inventory.Product);
            }
        }

        return products;
    }

    public async Task<bool> IsProductAvailableAsync(Guid productId, int quantity)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        return inventory?.Quantity >= quantity;
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

    public async Task<bool> UpdateStockQuantityAsync(Guid productId, int quantity, string operation)
    {
        try
        {
            await _inventoryRepository.UpdateStockAsync(productId, quantity, operation);

            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            await _auditService.LogActivityAsync(
                "Inventory",
                inventory!.Id,
                $"Stock{operation}",
                "system",
                oldValues: quantity.ToString(),
                newValues: inventory.Quantity.ToString());

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
