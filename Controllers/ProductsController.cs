using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    // maps the name of the controller to the URL
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        // do dependency injection for the service layer
        private readonly IProductService _productService = productService;

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        // mapped to a @RequestBody from Java - SpringBoot
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // we check if the product is null
            if (product is null)
            {
                return BadRequest();
            }
            var createdProduct = await _productService.CreateProductAsync(product);
            return Ok(createdProduct);
            //return CreatedAtAction(
            //    nameof(GetProduct), // name of the action method
            //    new { id = createdProduct.Id }, // route values
            //    createdProduct // response body
            //);
        }

        [HttpPut("{id}")]
        // we used the IActionResult to return a status code
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        // we used the IActionResult to return a status code -> only for DELETE and PUT requests
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
