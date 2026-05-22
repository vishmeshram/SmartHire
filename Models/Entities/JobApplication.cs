namespace SmartHire.Models.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Status { get; set; } = "Applied"; // Applied, Interview, Offer, Rejected
        public string? ApplicationUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DeadlineDate { get; set; }

        public User User { get; set; } = null!;
        public AIAnalysis? AIAnalysis { get; set; }
    }
}
