
namespace ResumeIQ.API.Models;

public class Resume
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string FileName { get; set; }
    public required string ExtractedText { get; set; }

    // Storing the nested Gemini AI JSON response directly in the database
    public string? AiAnalysisJson { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; }
}