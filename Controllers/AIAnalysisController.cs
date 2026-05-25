using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Helpers;
using SmartHire.Models.DTOs;
using SmartHire.Services.Interfaces;

namespace SmartHire.Controllers
{
    [ApiController]
    [Route("api/jobapplications/{applicationId}/analysis")]
    [Authorize]
    public class AIAnalysisController : ControllerBase
    {
        private readonly IAIAnalysisService _aiService;

        public AIAnalysisController(IAIAnalysisService aiService)
        {
            _aiService = aiService;
        }

        // POST api/jobapplications/1/analysis
        [HttpPost]
        public async Task<IActionResult> Analyze(int applicationId, AnalyzeResumeDTO dto)
        {
            try
            {
                var userId = ClaimsHelper.GetUserId(User);

                var result = await _aiService.AnalyzeAsync(applicationId, dto, userId);

                if (result == null)
                    return NotFound(new { message = "Application not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (to console, Serilog, etc.)
                Console.WriteLine($"Error analyzing resume: {ex}");

                // Return a descriptive error instead of a blank 500
                return StatusCode(500, new { message = "Analysis failed", detail = ex.Message });
            }
        }


        // GET api/jobapplications/1/analysis
        [HttpGet]
        public async Task<IActionResult> GetAnalysis(int applicationId)
        {
            var userId = ClaimsHelper.GetUserId(User);

            var result = await _aiService.GetByApplicationIdAsync(applicationId, userId);

            if (result == null)
                return NotFound(new { message = "No analysis found for this application." });

            return Ok(result);
        }
    }
}
