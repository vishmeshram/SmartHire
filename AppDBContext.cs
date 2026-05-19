using Microsoft.EntityFrameworkCore;
using SmartHire.Models.Entities;


namespace SmartHire.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User → JobApplications (one to many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.JobApplications)
                .WithOne(j => j.User)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // JobApplication → AIAnalysis (one to one)
            modelBuilder.Entity<JobApplication>()
                .HasOne(j => j.AIAnalysis)
                .WithOne(a => a.JobApplication)
                .HasForeignKey<AIAnalysis>(a => a.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}