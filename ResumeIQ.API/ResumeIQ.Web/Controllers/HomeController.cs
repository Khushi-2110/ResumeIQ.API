using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR.Protocol;
using ResumeIQ.Web.Models;
using System.Diagnostics;

namespace ResumeIQ.Web.Controllers;

[Authorize]

public class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile resumeFile)
    {
        if (resumeFile == null || resumeFile.Length == 0)
            return View("Index");

        using var client = new HttpClient();
        using var content = new MultipartFormDataContent();

        using var stream = resumeFile.OpenReadStream();
        content.Add(new StreamContent(stream), "file", resumeFile.FileName);

        // Sending to the API...
        var response = await client.PostAsync("https://localhost:7202/api/resume/upload", content);

        if (response.IsSuccessStatusCode)
        {
            // 1. Read the text from the API and store it in rawResult
            var rawResult = await response.Content.ReadAsStringAsync();

            // 2. Clean off the markdown formatting
            var cleanedJson = rawResult.Trim();
            if (cleanedJson.StartsWith("```json"))
            {
                cleanedJson = cleanedJson.Substring(7);
            }
            if (cleanedJson.StartsWith("```"))
            {
                cleanedJson = cleanedJson.Substring(3);
            }
            if (cleanedJson.EndsWith("```"))
            {
                cleanedJson = cleanedJson.Substring(0, cleanedJson.Length - 3);
            }
            cleanedJson = cleanedJson.Trim();

            // 3. Send the clean JSON to the View
            var model = new ResultViewModel { AiFeedback = cleanedJson };
            return View("Result", model);
        }
        // IF THE API FAILS, CAPTURE THE REAL ERROR AND SHOW IT IN THE CARD
        var errorDetails = await response.Content.ReadAsStringAsync();
        var errorModel = new ResultViewModel
        {
            AiFeedback = $"API Call Failed!\nStatus: {response.StatusCode}\nDetails: {errorDetails}"
        };

        return View("Result", errorModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult JobMatch()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AnalyzeJobMatch(IFormFile resumeFile, string jobDescription)
    {
        if (resumeFile == null || string.IsNullOrWhiteSpace(jobDescription))
            return RedirectToAction("JobMatch");

        using var client = new HttpClient();
        using var content = new MultipartFormDataContent();

        // Attach the file
        using var stream = resumeFile.OpenReadStream();
        content.Add(new StreamContent(stream), "file", resumeFile.FileName);

        // Attach the text
        content.Add(new StringContent(jobDescription), "jobDescription");

        // Send to your backend JobController. 
        // UPDATE THIS URL if your JobController route is different!
        var response = await client.PostAsync("https://localhost:7202/api/job/match", content);

        if (response.IsSuccessStatusCode)
        {
            var rawResult = await response.Content.ReadAsStringAsync();
            // For now, we will reuse the Result view to display the AI's matching logic
            return View("Result", new ResultViewModel { AiFeedback = rawResult });
        }

        return View("Result", new ResultViewModel { AiFeedback = $"Match Failed: {response.StatusCode}" });
    }
}
