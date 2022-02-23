using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OK.Siemens.DataProviders;

/// <summary>
/// For migrations in design time
/// </summary>
public class MigrationsContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5435;Database=siemens;Username=postgres;Password=dbpass");

        return new AppDbContext(optionsBuilder.Options);
    }
}