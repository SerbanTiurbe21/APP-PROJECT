using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        //This constructor is used to pass the options to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // This method is used to configure the database context
        // rule: The name of the DbSet should be plural
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
