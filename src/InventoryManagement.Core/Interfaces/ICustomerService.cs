using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ICustomerService
{
    Task<Customer> AddCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
    Task<bool> SendWhatsAppMessageAsync(Guid customerId, string message);
    Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm, Guid tenantId);
    Task<Customer?> GetCustomerByMobileAsync(string mobile, Guid tenantId);
    Task<IEnumerable<Sale>> GetCustomerPurchaseHistoryAsync(Guid customerId);
}
