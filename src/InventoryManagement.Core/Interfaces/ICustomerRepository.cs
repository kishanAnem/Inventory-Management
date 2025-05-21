using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IReadOnlyList<Customer>> GetCustomersByTenantAsync(Guid tenantId);
    Task<Customer?> GetByMobileNumberAsync(string mobileNumber, Guid tenantId);
    Task<IReadOnlyList<Customer>> SearchCustomersAsync(string searchTerm, Guid tenantId);
}
