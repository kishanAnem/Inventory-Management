using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<CustomerDTO>> GetAllAsync();
        Task<CustomerDTO> CreateAsync(CreateCustomerDTO createCustomerDto);
        Task<CustomerDTO> UpdateAsync(UpdateCustomerDTO updateCustomerDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<CustomerDTO>> SearchAsync(string searchTerm);
    }
}
