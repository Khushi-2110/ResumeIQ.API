using Google.GenAI;

namespace ResumeIQ.API.Services;

public class GeminiAiService
{
    private readonly Client _client;

    public GeminiAiService(IConfiguration config)
    {
        // Initializes the new unified Google Gen AI SDK client
        _client = new Client(apiKey: config["Gemini:ApiKey"]);
    }

    public async Task<string> AnalyzeResumeAsync(string resumeText)
    {
        var prompt = $"Analyze this resume and provide an ATS score in JSON format: {resumeText}";
        var response = await _client.Models.GenerateContentAsync("gemini-2.5-flash", prompt);
        return response.Text;
    }
}