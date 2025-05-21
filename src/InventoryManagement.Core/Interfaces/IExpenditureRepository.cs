using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IExpenditureRepository : IGenericRepository<Expenditure>
{
    Task<IReadOnlyList<Expenditure>> GetExpendituresByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalExpenditureAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Expenditure>> GetExpendituresByTypeAsync(Guid expenditureTypeId);
}
