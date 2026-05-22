using Microsoft.AspNetCore.Mvc;
using SmartHire.Models.DTOs;
using SmartHire.Services.Interfaces;


namespace SmartHire.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return Conflict(new { message = "Email already in use." });
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }
    }
}