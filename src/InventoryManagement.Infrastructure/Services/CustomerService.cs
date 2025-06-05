using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAuditService _auditService;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _customerRepository = (ICustomerRepository)unitOfWork.Repository<Customer>();
        _auditService = auditService;
    }

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        // Check if customer with same mobile exists for the tenant
        var existingCustomer = await _customerRepository.GetByMobileNumberAsync(customer.Mobile, customer.TenantId);
        if (existingCustomer != null)
        {
            throw new InvalidOperationException($"Customer with mobile number {customer.Mobile} already exists");
        }

        var addedCustomer = await _customerRepository.AddAsync(customer);
        
        await _auditService.LogActivityAsync(
            "Customer",
            addedCustomer.Id,
            "Create",
            customer.CreatedBy);

        return addedCustomer;
    }

    public async Task<Customer?> GetCustomerByMobileAsync(string mobile, Guid tenantId)
    {
        return await _customerRepository.GetByMobileNumberAsync(mobile, tenantId);
    }

    public async Task<IEnumerable<Sale>> GetCustomerPurchaseHistoryAsync(Guid customerId)
    {
        var saleRepository = (ISaleRepository)_unitOfWork.Repository<Sale>();
        return await saleRepository.GetSalesByCustomerAsync(customerId);
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm, Guid tenantId)
    {
        return await _customerRepository.SearchCustomersAsync(searchTerm, tenantId);
    }

    public async Task<bool> SendWhatsAppMessageAsync(Guid customerId, string message)
    {
        // TODO: Implement WhatsApp integration
        throw new NotImplementedException("WhatsApp integration not implemented yet");
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(customer.Id)
            ?? throw new InvalidOperationException($"Customer with ID {customer.Id} not found.");

        var oldValues = System.Text.Json.JsonSerializer.Serialize(existingCustomer);
        
        await _customerRepository.UpdateAsync(customer);
        
        await _auditService.LogActivityAsync(
            "Customer",
            customer.Id,
            "Update",
            customer.LastModifiedBy ?? "system",
            oldValues,
            System.Text.Json.JsonSerializer.Serialize(customer));
    }
}
