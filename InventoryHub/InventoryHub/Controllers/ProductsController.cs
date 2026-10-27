
using Microsoft.AspNetCore.Mvc;
using InventoryHub.Models;
using InventoryHub.Services;
using Microsoft.AspNetCore.Authorization;

namespace InventoryHub.Controllers
{
        [ApiController]
        [Route("api/[controller]")] // Attribute Routing
        [Authorize] // Requires valid authentication token
        public class ProductsController : ControllerBase
        {
            private readonly IProductApiService _service;
            private readonly ILogger<ProductsController> _logger;

            public ProductsController(IProductApiService service, ILogger<ProductsController> logger)
            {
                _service = service;
                _logger = logger;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                try
                {
                    var products = await _service.GetAllProductsAsync();
                    return Ok(products);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching products.");
                    return StatusCode(500, "Internal server error");
                }
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var product = await _service.GetProductByIdAsync(id);
                if (product == null) return NotFound();
                return Ok(product);
            }

            [HttpPost]
            [Authorize(Roles = "Admin,Manager")] // Authorization Mechanism
            public async Task<IActionResult> Create([FromBody] Product product)
            {
                if (!ModelState.IsValid) return BadRequest(ModelState); // Input Validation

                var createdProduct = await _service.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }

            [HttpPut("{id}")]
            [Authorize(Roles = "Admin,Manager")]
            public async Task<IActionResult> Update(int id, [FromBody] Product product)
            {
                if (id != product.Id) return BadRequest("ID mismatch");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                await _service.UpdateProductAsync(product);
                return NoContent();
            }

            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(int id)
            {
                await _service.DeleteProductAsync(id);
                return NoContent();
            }
        }
    
}
