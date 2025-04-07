using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDTO user);

        // ? is used to return a nullable value
        Task<string?> LoginAsync(UserDTO user);
    }
}
