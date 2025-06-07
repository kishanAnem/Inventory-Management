using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class ProductsController : ApiControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return HandleSuccess(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, new { message = "An error occurred while retrieving products" });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return HandleSuccess(product);
            }
            catch (KeyNotFoundException ex)
            {
                return HandleNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the product" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO createProductDto)
        {
            try
            {
                var product = await _productService.CreateAsync(createProductDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "An error occurred while creating the product" });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDTO updateProductDto)
        {
            if (id != updateProductDto.Id)
                return HandleBadRequest("ID mismatch");

            try
            {
                var product = await _productService.UpdateAsync(updateProductDto);
                return HandleSuccess(product);
            }
            catch (KeyNotFoundException ex)
            {
                return HandleNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the product" });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (!result)
                    return HandleNotFoundException($"Product with ID {id} not found");

                return HandleSuccess(message: "Product deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the product" });
            }
        }

        [HttpPut("{id:guid}/stock")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStock(Guid id, [FromBody] int quantity)
        {
            try
            {
                if (quantity < 0)
                    return HandleBadRequest("Stock quantity cannot be negative");

                await _productService.UpdateStockQuantityAsync(id, quantity);
                return HandleSuccess(message: "Stock updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return HandleNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product with ID: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the stock" });
            }
        }
    }
}
