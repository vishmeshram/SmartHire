using SmartHire.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using SmartHire.Helpers;

namespace SmartHire.Models.DTOs
{
    public class CreateJobApplicationDTO
    {
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(150, MinimumLength = 1)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(150, MinimumLength = 1)]
        public string JobTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required.")]
        [MinLength(20, ErrorMessage = "Please paste the full job description (min 20 characters).")]
        public string JobDescription { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ApplicationUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }

        [FutureDate(ErrorMessage = "Deadline must be a future date.")]
        public DateTime? DeadlineDate { get; set; }
    }

    public class UpdateStatusDTO
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Applied|Interview|Offer|Rejected)$",
            ErrorMessage = "Status must be Applied, Interview, Offer, or Rejected.")]
        public string Status { get; set; } = string.Empty;
    }

    public class JobApplicationResponseDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ApplicationUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime AppliedDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public AIAnalysisResponseDTO? AIAnalysis { get; set; }
    }
}