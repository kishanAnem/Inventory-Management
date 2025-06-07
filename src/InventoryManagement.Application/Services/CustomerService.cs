using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IWhatsAppService _whatsAppService;

        public CustomerService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ICacheService cacheService,
            IWhatsAppService whatsAppService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _whatsAppService = whatsAppService;
        }

        public async Task<CustomerDTO> GetByIdAsync(Guid id)
        {
            var cacheKey = $"customer_{id}";
            var cachedCustomer = await _cacheService.GetAsync<CustomerDTO>(cacheKey);
            if (cachedCustomer != null)
                return cachedCustomer;

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {id} not found");

            var customerDto = _mapper.Map<CustomerDTO>(customer);
            await _cacheService.SetAsync(cacheKey, customerDto, TimeSpan.FromMinutes(30));

            return customerDto;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers
                .GetAll()
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> CreateAsync(CreateCustomerDTO createCustomerDto)
        {
            var customer = _mapper.Map<Customer>(createCustomerDto);
            
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            // Send welcome message via WhatsApp if phone number is provided
            if (!string.IsNullOrEmpty(customer.Phone))
            {
                await _whatsAppService.SendTemplateMessageAsync(
                    customer.Phone,
                    "welcome_message",
                    new { name = customer.Name }
                );
            }

            return await GetByIdAsync(customer.Id);
        }

        public async Task<CustomerDTO> UpdateAsync(UpdateCustomerDTO updateCustomerDto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(updateCustomerDto.Id);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {updateCustomerDto.Id} not found");

            _mapper.Map(updateCustomerDto, customer);
            
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"customer_{customer.Id}");

            return await GetByIdAsync(customer.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return false;

            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"customer_{id}");

            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _unitOfWork.Customers
                .GetAll()
                .AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CustomerDTO>> SearchAsync(string searchTerm)
        {
            var customers = await _unitOfWork.Customers
                .GetAll()
                .Where(c => 
                    c.Name.Contains(searchTerm) || 
                    c.Email.Contains(searchTerm) || 
                    c.Phone.Contains(searchTerm))
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
        }
    }
}
