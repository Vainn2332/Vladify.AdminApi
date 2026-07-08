using Microsoft.EntityFrameworkCore;
using Vladify.Domain.Entities;
using Vladify.Infrastructure.Configurations;

namespace Vladify.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ModerationTask> ModerationTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ModerationTaskConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
