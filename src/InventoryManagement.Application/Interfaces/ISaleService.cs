using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces
{
    public interface ISaleService
    {
        Task<SaleDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<SaleDTO>> GetAllAsync();
        Task<SaleDTO> CreateAsync(CreateSaleDTO createSaleDto);
        Task<bool> CancelAsync(Guid id);
        Task<IEnumerable<SaleDTO>> GetCustomerSalesAsync(Guid customerId);
        Task<IEnumerable<SaleDTO>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate);
    }
}
