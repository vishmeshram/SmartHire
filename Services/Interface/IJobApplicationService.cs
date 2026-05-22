using SmartHire.Models.DTOs;


namespace SmartHire.Services.Interfaces
{
    public interface IJobApplicationService
    {
        Task<List<JobApplicationResponseDTO>> GetAllAsync(int userId);
        Task<JobApplicationResponseDTO?> GetByIdAsync(int id, int userId);
        Task<JobApplicationResponseDTO> CreateAsync(CreateJobApplicationDTO dto, int userId);
        Task<JobApplicationResponseDTO?> UpdateStatusAsync(int id, UpdateStatusDTO dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}

