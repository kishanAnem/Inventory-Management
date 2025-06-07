using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class SalesController : ApiControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(
            ISaleService saleService,
            ILogger<SalesController> logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var sales = await _saleService.GetAllAsync();
                return HandleSuccess(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sales");
                return StatusCode(500, new { message = "An error occurred while retrieving sales" });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var sale = await _saleService.GetByIdAsync(id);
                return HandleSuccess(sale);
            }
            catch (KeyNotFoundException ex)
            {
                return HandleNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale with ID: {SaleId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the sale" });
            }
        }

        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetCustomerSales(Guid customerId)
        {
            try
            {
                var sales = await _saleService.GetCustomerSalesAsync(customerId);
                return HandleSuccess(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales for customer ID: {CustomerId}", customerId);
                return StatusCode(500, new { message = "An error occurred while retrieving customer sales" });
            }
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
                var totalAmount = await _saleService.GetTotalSalesAmountAsync(startDate, endDate);

                return HandleSuccess(new
                {
                    sales,
                    totalAmount,
                    startDate,
                    endDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales report from {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, new { message = "An error occurred while generating the sales report" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleDTO createSaleDto)
        {
            try
            {
                var sale = await _saleService.CreateAsync(createSaleDto);
                return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
            }
            catch (InvalidOperationException ex)
            {
                return HandleBadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return StatusCode(500, new { message = "An error occurred while creating the sale" });
            }
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var result = await _saleService.CancelAsync(id);
                if (!result)
                    return HandleNotFoundException($"Sale with ID {id} not found");

                return HandleSuccess(message: "Sale cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling sale with ID: {SaleId}", id);
                return StatusCode(500, new { message = "An error occurred while cancelling the sale" });
            }
        }
    }
}
