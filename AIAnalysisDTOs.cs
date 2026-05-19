using System.ComponentModel.DataAnnotations;

namespace SmartHire.Models.DTOs
{
    public class AnalyzeResumeDTO
    {
        [Required(ErrorMessage = "Resume text is required.")]
        [MinLength(100, ErrorMessage = "Please paste your full resume (min 100 characters).")]
        public string ResumeText { get; set; } = string.Empty;
    }

    public class AIAnalysisResponseDTO
    {
        public int MatchScore { get; set; }
        public string ResumeAnalysis { get; set; } = string.Empty;
        public string RewrittenBullets { get; set; } = string.Empty;
        public string ColdEmail { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}