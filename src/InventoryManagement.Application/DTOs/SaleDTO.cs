using InventoryManagement.Application.Common.DTOs;

namespace InventoryManagement.Application.DTOs
{
    public class SaleDTO : BaseDTO
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public List<SaleItemDTO> Items { get; set; } = new();
    }

    public class SaleItemDTO : BaseDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CreateSaleDTO
    {
        public Guid CustomerId { get; set; }
        public List<CreateSaleItemDTO> Items { get; set; } = new();
    }

    public class CreateSaleItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
