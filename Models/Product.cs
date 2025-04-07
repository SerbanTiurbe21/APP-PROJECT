namespace WebApplication1.Models
{
    public class Product
    {
        public Guid Id { get; set; }     // Primary Key
        public required string Name { get; set; }
    }
}
