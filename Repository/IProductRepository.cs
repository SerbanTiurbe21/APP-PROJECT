using WebApplication1.Models;

namespace WebApplication1.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(Guid id);

        Task<Product> CreateProductAsync(Product product);

        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(Guid id);
        Task<bool> ProductExistsAsync(string name);
    }
}
