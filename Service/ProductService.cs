using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Repository;

namespace WebApplication1.Service
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {

        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Product> CreateProductAsync(Product product)
        {
            bool productExists = await _productRepository.ProductExistsAsync(product.Name);
            if (productExists)
            {
                throw new DuplicateProductException("Product already exists");
            }
            return await _productRepository.CreateProductAsync(product);
        }
        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id) ?? throw new Exception("Product not found");
            await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id) ?? throw new Exception("Product not found");
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(product.Id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }
            await _productRepository.UpdateProductAsync(product);
        }
    }
}
