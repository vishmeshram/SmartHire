namespace SmartHire.Models.Entities
{
    public class AIAnalysis
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public int MatchScore { get; set; }          // 0-100
        public string ResumeAnalysis { get; set; } = string.Empty;
        public string RewrittenBullets { get; set; } = string.Empty;
        public string ColdEmail { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public JobApplication JobApplication { get; set; } = null!;
    }
}
