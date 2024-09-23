using Microsoft.EntityFrameworkCore;

namespace SatisfactorySaveEditor.Data;

public class AppSettingsDbContext : DbContext
{
    public AppSettingsDbContext(DbContextOptions<AppSettingsDbContext> options)
        : base(options) => Database.EnsureCreated();

    public DbSet<AppSettings> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<AppSettings>().HasData(SeedSettings());
        base.OnModelCreating(modelBuilder);
    }

    private static List<AppSettings> SeedSettings() =>
    [
        new AppSettings
        {
            Id = 1,
            LastSaves = [],
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowLeft = -1,
            WindowTop = -1,
            AutoUpdate = true,
            AutoBackup = true
        },
    ];
}
