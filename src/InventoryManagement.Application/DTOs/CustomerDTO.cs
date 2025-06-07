using InventoryManagement.Application.Common.DTOs;

namespace InventoryManagement.Application.DTOs
{
    public class CustomerDTO : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public Guid TenantId { get; set; }
    }

    public class CreateCustomerDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
    }

    public class UpdateCustomerDTO : CreateCustomerDTO
    {
        public Guid Id { get; set; }
    }
}
