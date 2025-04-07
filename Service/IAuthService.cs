using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto user);

        // ? is used to return a nullable value
        Task<TokenResponseDto?> LoginAsync(UserDto user);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
