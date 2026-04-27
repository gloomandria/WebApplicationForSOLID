using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProjetScolariteSOLID.Infrastructure.Data;

/// <summary>
/// Factory pour dotnet ef CLI.
/// Charge les paramètres depuis appsettings.json via IConfigurationBuilder.
/// </summary>
public sealed class ScolariteDbContextFactory : IDesignTimeDbContextFactory<ScolariteDbContext>
{
    public ScolariteDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Chercher le dossier WebApplicationForSOLID contenant appsettings.json
        string? webAppDir = null;
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "WebApplicationForSOLID"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "WebApplicationForSOLID"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "WebApplicationForSOLID"),
        };
        foreach (var c in candidates)
        {
            if (File.Exists(Path.Combine(c, "appsettings.json"))) { webAppDir = c; break; }
        }
        if (webAppDir is null)
            throw new InvalidOperationException("Impossible de localiser appsettings.json.");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(webAppDir)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("ScolariteDb")
            ?? throw new InvalidOperationException("ConnectionString 'ScolariteDb' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ScolariteDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ScolariteDbContext(optionsBuilder.Options);
    }
}
