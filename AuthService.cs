using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartHire.Data;
using SmartHire.Models.DTOs;
using SmartHire.Models.Entities;
using SmartHire.Services.Interfaces;


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SmartHire.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto)
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (existingUser != null)
                return null; // Email taken

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Token = GenerateJwtToken(user),
                FullName = user.FullName,
                Email = user.Email
            };
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return new AuthResponseDTO
            {
                Token = GenerateJwtToken(user),
                FullName = user.FullName,
                Email = user.Email
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(
                    int.Parse(jwtSettings["ExpiryDays"]!)),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
