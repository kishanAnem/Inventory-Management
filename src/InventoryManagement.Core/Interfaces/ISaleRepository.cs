using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ISaleRepository : IGenericRepository<Sale>
{
    Task<IReadOnlyList<Sale>> GetSalesByCustomerAsync(Guid customerId);
    Task<IReadOnlyList<Sale>> GetSalesByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalSalesAsync(Guid tenantId, DateTime startDate, DateTime endDate);
}
