using SmartHire.Models.DTOs;


namespace SmartHire.Services.Interfaces
{
    public interface IAIAnalysisService
    {
        Task<AIAnalysisResponseDTO?> AnalyzeAsync(int applicationId, AnalyzeResumeDTO dto, int userId);
        Task<AIAnalysisResponseDTO?> GetByApplicationIdAsync(int applicationId, int userId);
    }
}