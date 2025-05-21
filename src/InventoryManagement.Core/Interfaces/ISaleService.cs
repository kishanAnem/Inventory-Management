using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ISaleService
{
    Task<Sale> CreateSaleAsync(Sale sale);
    Task<bool> ProcessPaymentAsync(Sale sale, string paymentMethod);
    Task<string> GenerateInvoiceNumberAsync(Guid tenantId);
    Task<byte[]> GenerateInvoicePdfAsync(Guid saleId);
    Task<IEnumerable<Sale>> GetSalesReportAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<decimal> CalculateTotalRevenueAsync(Guid tenantId, DateTime startDate, DateTime endDate);
}
