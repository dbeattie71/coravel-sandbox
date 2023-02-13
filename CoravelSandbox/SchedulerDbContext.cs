using Coravel.Pro.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Scheduler;

public class SchedulerDbContext : DbContext, ICoravelProDbContext
{
    public SchedulerDbContext()
    {
    }

    public SchedulerDbContext(
        DbContextOptions<SchedulerDbContext> options
    )
        : base(options)
    {
    }

    public DbSet<CoravelJobHistory> Coravel_JobHistory { get; set; } = null!;
    public DbSet<CoravelScheduledJob> Coravel_ScheduledJobs { get; set; } = null!;
    public DbSet<CoravelScheduledJobHistory> Coravel_ScheduledJobHistory { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
    }
}