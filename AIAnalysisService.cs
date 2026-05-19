using Microsoft.EntityFrameworkCore;

using SmartHire.Data;
using SmartHire.Models.DTOs;
using SmartHire.Models.Entities;
using SmartHire.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace SmartHire.Services
{
    public class AIAnalysisService : IAIAnalysisService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AIAnalysisService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<AIAnalysisResponseDTO?> AnalyzeAsync(
            int applicationId, AnalyzeResumeDTO dto, int userId)
        {
            var application = await _context.JobApplications
                .Include(j => j.AIAnalysis)
                .FirstOrDefaultAsync(j => j.Id == applicationId && j.UserId == userId);

            if (application == null) return null;

            var analysis = await CallGroqAsync(application.JobDescription, dto.ResumeText);

            if (application.AIAnalysis != null)
            {
                application.AIAnalysis.MatchScore = analysis.MatchScore;
                application.AIAnalysis.ResumeAnalysis = analysis.ResumeAnalysis;
                application.AIAnalysis.RewrittenBullets = analysis.RewrittenBullets;
                application.AIAnalysis.ColdEmail = analysis.ColdEmail;
                application.AIAnalysis.GeneratedAt = DateTime.UtcNow;
            }
            else
            {
                application.AIAnalysis = new AIAnalysis
                {
                    JobApplicationId = applicationId,
                    MatchScore = analysis.MatchScore,
                    ResumeAnalysis = analysis.ResumeAnalysis,
                    RewrittenBullets = analysis.RewrittenBullets,
                    ColdEmail = analysis.ColdEmail
                };
            }

            await _context.SaveChangesAsync();
            return analysis;
        }

        public async Task<AIAnalysisResponseDTO?> GetByApplicationIdAsync(
            int applicationId, int userId)
        {
            var application = await _context.JobApplications
                .Include(j => j.AIAnalysis)
                .FirstOrDefaultAsync(j => j.Id == applicationId && j.UserId == userId);

            if (application?.AIAnalysis == null) return null;

            return new AIAnalysisResponseDTO
            {
                MatchScore = application.AIAnalysis.MatchScore,
                ResumeAnalysis = application.AIAnalysis.ResumeAnalysis,
                RewrittenBullets = application.AIAnalysis.RewrittenBullets,
                ColdEmail = application.AIAnalysis.ColdEmail,
                GeneratedAt = application.AIAnalysis.GeneratedAt
            };
        }

        private async Task<AIAnalysisResponseDTO> CallGroqAsync(
            string jobDescription, string resumeText)
        {
            var apiKey = _config["Groq:ApiKey"];

            var jsonFormat = @"{
                ""matchScore"": <number 0-100>,
                ""resumeAnalysis"": ""<3-4 sentences on strengths and gaps>"",
                ""rewrittenBullets"": ""<3-5 improved bullet points tailored to this job, each starting with a bullet>"",
                ""coldEmail"": ""<a professional cold email to the hiring manager, max 150 words>""
            }";

            var prompt = $"You are an expert career coach and resume analyst.\n\n" +
                        $"Analyze this resume against the job description and respond in this EXACT JSON format, nothing else:\n" +
                        jsonFormat + "\n\n" +
                        $"JOB DESCRIPTION:\n{jobDescription}\n\n" +
                        $"RESUME:\n{resumeText}\n\n" +
                        "Respond ONLY with the JSON object. No markdown, no explanation, no code blocks.";

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.3
            };

            var json = JsonSerializer.Serialize(requestBody);

            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Groq API error: {responseBody}");

            // Parse Groq response
            var groqResponse = JsonDocument.Parse(responseBody);
            var content = groqResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            // Strip markdown if model adds it anyway
            content = content
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var result = JsonDocument.Parse(content);
            var root = result.RootElement;

            return new AIAnalysisResponseDTO
            {
                MatchScore = root.GetProperty("matchScore").GetInt32(),
                ResumeAnalysis = root.GetProperty("resumeAnalysis").GetString() ?? "",
                RewrittenBullets = root.GetProperty("rewrittenBullets").GetString() ?? "",
                ColdEmail = root.GetProperty("coldEmail").GetString() ?? "",
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}