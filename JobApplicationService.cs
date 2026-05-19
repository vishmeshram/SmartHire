using Microsoft.EntityFrameworkCore;
using SmartHire.Data;
using SmartHire.Models.DTOs;
using SmartHire.Models.Entities;
using SmartHire.Services.Interfaces;

namespace SmartHire.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly AppDbContext _context;

        public JobApplicationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobApplicationResponseDTO>> GetAllAsync(int userId)
        {
            return await _context.JobApplications
                .Where(j => j.UserId == userId)
                .Include(j => j.AIAnalysis)
                .OrderByDescending(j => j.AppliedDate)
                .Select(j => MapToResponseDTO(j))
                .ToListAsync();
        }

        public async Task<JobApplicationResponseDTO?> GetByIdAsync(int id, int userId)
        {
            var application = await _context.JobApplications
                .Include(j => j.AIAnalysis)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (application == null) return null;

            return MapToResponseDTO(application);
        }

        public async Task<JobApplicationResponseDTO> CreateAsync(
            CreateJobApplicationDTO dto, int userId)
        {
            var application = new JobApplication
            {
                UserId = userId,
                CompanyName = dto.CompanyName,
                JobTitle = dto.JobTitle,
                JobDescription = dto.JobDescription,
                ApplicationUrl = dto.ApplicationUrl,
                Notes = dto.Notes,
                DeadlineDate = dto.DeadlineDate,
                Status = "Applied"
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            return MapToResponseDTO(application);
        }

        public async Task<JobApplicationResponseDTO?> UpdateStatusAsync(
            int id, UpdateStatusDTO dto, int userId)
        {
            var application = await _context.JobApplications
                .Include(j => j.AIAnalysis)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (application == null) return null;

            application.Status = dto.Status;
            await _context.SaveChangesAsync();

            return MapToResponseDTO(application);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var application = await _context.JobApplications
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

            if (application == null) return false;

            _context.JobApplications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        private static JobApplicationResponseDTO MapToResponseDTO(JobApplication j)
        {
            return new JobApplicationResponseDTO
            {
                Id = j.Id,
                CompanyName = j.CompanyName,
                JobTitle = j.JobTitle,
                JobDescription = j.JobDescription,
                Status = j.Status,
                ApplicationUrl = j.ApplicationUrl,
                Notes = j.Notes,
                AppliedDate = j.AppliedDate,
                DeadlineDate = j.DeadlineDate,
                AIAnalysis = j.AIAnalysis == null ? null : new AIAnalysisResponseDTO
                {
                    MatchScore = j.AIAnalysis.MatchScore,
                    ResumeAnalysis = j.AIAnalysis.ResumeAnalysis,
                    RewrittenBullets = j.AIAnalysis.RewrittenBullets,
                    ColdEmail = j.AIAnalysis.ColdEmail,
                    GeneratedAt = j.AIAnalysis.GeneratedAt
                }
            };
        }
    }
}
