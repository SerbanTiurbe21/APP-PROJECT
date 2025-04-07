using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Exceptions;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    // we can inject the db context in the constructor
    public class ProductService(AppDbContext context) : IProductService
    {

        // inject the db context 
        private readonly AppDbContext _context = context;

        public async Task<Product> CreateProductAsync(Product product)
        {
            bool productExists = await _context.Products.AnyAsync(p => p.Name.ToLower() == product.Name.ToLower());
            if (productExists)
            {
                throw new DuplicateProductException("Product already exists");
            }
            // we add the product to the db context
            _context.Products.Add(product);
            // we save the changes to the database
            await _context.SaveChangesAsync();
            // we return the product
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id) ?? throw new Exception("Product not found");
            // we remove the product from the db context
            _context.Products.Remove(product);
            // we save the changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id) => await _context.Products.FindAsync(id) ?? throw new Exception("Product not found");
        
        public async Task UpdateProductAsync(Product product)
        {
            // we check if the product exists in the database
            // This is a discard operator in C#. It indicates that the result of the expression is intentionally ignored.
            // In this case, the result of FindAsync is not stored in a variable because it is only used to check for existence.
            _ = await _context.Products.FindAsync(product.Id) ?? throw new Exception("Product not found");
            // this line is used to inform the db context that the product has been modified
            _context.Entry(product).State = EntityState.Modified;
            // we save the changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
