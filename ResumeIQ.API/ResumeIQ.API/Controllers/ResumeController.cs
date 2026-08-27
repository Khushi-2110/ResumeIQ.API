using Microsoft.AspNetCore.Mvc;
using ResumeIQ.API.Models;
using ResumeIQ.API.Services;
using System.Text;
using UglyToad.PdfPig;

namespace ResumeIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumeController : ControllerBase
{
    private readonly GeminiAiService _aiService;
    private readonly ApplicationDbContext _db;

    public ResumeController(GeminiAiService aiService, ApplicationDbContext db)
    {
        _aiService = aiService;
        _db = db;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Invalid file");

        var extractedText = new StringBuilder();

        // 1. Read the uploaded PDF into memory (prevents disk storage leakage)
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // 2. Extract text using PdfPig
            using (var document = PdfDocument.Open(memoryStream))
            {
                foreach (var page in document.GetPages())
                {
                    extractedText.Append(page.Text).Append(" ");
                }
            }
        }

        // 3. Send text to Gemini AI
        var rawText = extractedText.ToString();
        var aiJsonResponse = await _aiService.AnalyzeResumeAsync(rawText);

        // 4. Save to SQL Database (Simulating a logged-in user with UserId = 1 for now)
        var resumeRecord = new Resume
        {
            UserId = 1,
            FileName = file.FileName,
            ExtractedText = rawText,
            AiAnalysisJson = aiJsonResponse
        };

        _db.Resumes.Add(resumeRecord);
        await _db.SaveChangesAsync();

        // 5. Return JSON payload to React frontend
        return Content(aiJsonResponse, "application/json");
    }
}