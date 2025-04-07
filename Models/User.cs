namespace WebApplication1.Models
{
    public class User
    {
        public Guid Id { get; set; }
        // here we make the username by setting the username to be Empty if it is null
        public String Username { get; set; } = String.Empty;
        public String PasswordHash { get; set; } = String.Empty;
    }
}
