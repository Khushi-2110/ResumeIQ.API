using Microsoft.EntityFrameworkCore;
using ResumeIQ.API.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Entity Framework Core with an In-Memory Database for testing tonight
// (You can swap this to UseSqlServer later)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ResumeIQDb"));

// 2. Add CORS so your React frontend (localhost:5173) can communicate with this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ResumeIQ.API.Services.GeminiAiService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 3. Apply CORS before Authorization and Mapping Controllers
app.UseCors("AllowReactApp");

app.UseAuthorization();
app.MapControllers();

// --- DATABASE SEEDER FOR EVALUATION DEMO ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ResumeIQ.API.Models.ApplicationDbContext>();

    // This safely creates the tables if they were wiped
    db.Database.EnsureCreated();

    // Check if our permanent demo user exists
    if (!db.Users.Any(u => u.Email == "demo@resumeiq.com"))
    {
        db.Users.Add(new ResumeIQ.API.Models.User
        {
            FullName = "Capstone Evaluator",
            Email = "demo@resumeiq.com",
            PasswordHash = "admin123", // Easy password for the demo
            Role = "job_seeker",
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}
app.Run();