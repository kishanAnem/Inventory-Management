using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult HandleNotFoundException(string message)
        {
            return NotFound(new { message });
        }

        protected IActionResult HandleBadRequest(string message)
        {
            return BadRequest(new { message });
        }

        protected IActionResult HandleSuccess(object? data = null, string? message = null)
        {
            return Ok(new
            {
                success = true,
                message,
                data
            });
        }
    }
}
