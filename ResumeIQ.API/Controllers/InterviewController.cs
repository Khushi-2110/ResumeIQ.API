using Microsoft.AspNetCore.Mvc;
using ResumeIQ.API.Services;

namespace ResumeIQ.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly GeminiAiService _aiService;

        public InterviewController(GeminiAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("generate-questions")]
        public async Task<IActionResult> GenerateQuestions([FromBody] string resumeText)
        {
            if (string.IsNullOrEmpty(resumeText))
                return BadRequest("Resume text is required.");

            var prompt = $"Based on this resume, generate 3 Technical interview questions, 2 Project-specific questions, and 2 HR/Behavioral questions.\n\nResume: {resumeText}";

            var result = await _aiService.GenerateContentAsync(prompt);
            return Ok(new { Questions = result });
        }
    }
}
