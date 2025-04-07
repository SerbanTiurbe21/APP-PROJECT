using WebApplication1.Models;

namespace WebApplication1.Service
{
    public interface IProductService
    {
        // we use async and await to make the code non-blocking
        // we use Task to return a value in the future
        // we use IEnumerable to return a collection of products
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // we use ? to return a nullable value
        Task<Product?> GetProductByIdAsync(int id);

        Task<Product> CreateProductAsync(Product product);

        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(int id);
    }
}
