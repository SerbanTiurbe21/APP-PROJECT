using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.DTO;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // do dependency injection for the service layer
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("User already exists");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        { 
            var token = await _authService.LoginAsync(request);
            if (token is null)
            {
                return BadRequest("Wrong password");
            }
            return Ok(token);
        }
    }
}
