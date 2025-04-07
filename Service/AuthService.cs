using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    // we pass in the configuration in the constructor
    // it is used to access the appsettings.json file
    public class AuthService(IConfiguration configuration, AppDbContext appDbContext) : IAuthService
    {
        private string CreateToken(User user)
        {
            // Creates a list of claims for the JWT token. In this case, it adds a single claim for the user's username.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            // Retrieves the Token value from the configuration and creates a symmetric security key using that value.
            // The key is used to sign the JWT token.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            // Creates signing credentials using the security key and the HMAC SHA-512 algorithm. These credentials are used to sign the JWT token.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Creates a JwtSecurityToken object with the following parameters:
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return null;
            }
            // here we check if the password is correct
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return await GenerateTokenResponseAsync(user);
        }

        private async Task<TokenResponseDto> GenerateTokenResponseAsync(User? user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user!),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user!)
            };
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            // first we check if the user already exists
            if (await appDbContext.Users.AnyAsync(u => u.Username == request.Username))
            { 
                return null;
            }
            var user = new User();
            var hassedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.Username = request.Username;
            user.PasswordHash = hassedPassword;

            // we add the user to the database
            await appDbContext.Users.AddAsync(user);
            // we save the changes to the database
            await appDbContext.SaveChangesAsync();
            // we return the user
            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                return null;
            }
            return await GenerateTokenResponseAsync(user);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await appDbContext.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        { 
            var user = await appDbContext.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
    }
}
