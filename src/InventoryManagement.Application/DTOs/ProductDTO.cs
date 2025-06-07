using InventoryManagement.Application.Common.DTOs;

namespace InventoryManagement.Application.DTOs
{
    public class ProductDTO : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryTypeId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }

    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryTypeId { get; set; }
    }

    public class UpdateProductDTO : CreateProductDTO
    {
        public Guid Id { get; set; }
    }
}
