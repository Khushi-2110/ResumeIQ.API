using Microsoft.EntityFrameworkCore;

namespace ResumeIQ.API.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Resume> Resumes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enforce unique emails
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}