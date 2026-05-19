using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHire.Helpers;
using SmartHire.Models.DTOs;
using SmartHire.Services.Interfaces;

namespace SmartHire.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _jobAppService;

        public JobApplicationsController(IJobApplicationService jobAppService)
        {
            _jobAppService = jobAppService;
        }

        // GET api/jobapplications
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var applications = await _jobAppService.GetAllAsync(userId);
            return Ok(applications);
        }

        // GET api/jobapplications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var application = await _jobAppService.GetByIdAsync(id, userId);

            if (application == null)
                return NotFound(new { message = "Application not found." });

            return Ok(application);
        }

        // POST api/jobapplications
        [HttpPost]
        public async Task<IActionResult> Create(CreateJobApplicationDTO dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var application = await _jobAppService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById),
                new { id = application.Id }, application);
        }

        // PATCH api/jobapplications/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDTO dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var application = await _jobAppService.UpdateStatusAsync(id, dto, userId);

            if (application == null)
                return NotFound(new { message = "Application not found." });

            return Ok(application);
        }

        // DELETE api/jobapplications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var deleted = await _jobAppService.DeleteAsync(id, userId);

            if (!deleted)
                return NotFound(new { message = "Application not found." });

            return NoContent();
        }
    }
}
