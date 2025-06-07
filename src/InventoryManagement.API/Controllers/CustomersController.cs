using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class CustomersController : ApiControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerService customerService,
            ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await _customerService.GetAllAsync();
                return HandleSuccess(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                return StatusCode(500, new { message = "An error occurred while retrieving customers" });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                return HandleSuccess(customer);
            }
            catch (KeyNotFoundException ex)
            {
                return HandleNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with ID: {CustomerId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the customer" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDTO createCustomerDto)
        {
            try
            {
                var customer = await _customerService.CreateAsync(createCustomerDto);
                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, new { message = "An error occurred while creating the customer" });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDTO updateCustomerDto)
        {
            if (id != updateCustomerDto.Id)
                return HandleBadRequest("ID mismatch");

            try
            {
                var customer = await _customerService.UpdateAsync(updateCustomerDto);
                return HandleSuccess(customer);
            }
            catch (KeyNotFoundException ex)
            {
                return HandleNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the customer" });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _customerService.DeleteAsync(id);
                if (!result)
                    return HandleNotFoundException($"Customer with ID {id} not found");

                return HandleSuccess(message: "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the customer" });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            try
            {
                var customers = await _customerService.SearchAsync(searchTerm);
                return HandleSuccess(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new { message = "An error occurred while searching customers" });
            }
        }
    }
}
