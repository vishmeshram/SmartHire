using SmartHire.Models.DTOs;


namespace SmartHire.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
    }
}
