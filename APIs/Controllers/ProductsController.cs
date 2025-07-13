using APIs.Entities.Interfaces;
using APIs.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _unitOfWork.Products.GetActiveProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string searchTerm)
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
            return Ok(products);
        }

        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<Product>> GetProductBySku(string sku)
        {
            var product = await _unitOfWork.Products.GetBySkuAsync(sku);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                SKU = request.SKU,
                CategoryId = request.CategoryId,
                StockQuantity = request.StockQuantity,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.SKU = request.SKU;
            product.CategoryId = request.CategoryId;
            product.StockQuantity = request.StockQuantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _unitOfWork.Products.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int CategoryId { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int CategoryId { get; set; }
        public int StockQuantity { get; set; }
    }
}