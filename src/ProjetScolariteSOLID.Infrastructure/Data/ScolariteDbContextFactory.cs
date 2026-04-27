using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjetScolariteSOLID.Infrastructure.Data;

/// <summary>
/// Factory pour dotnet ef CLI.
/// Charge les paramètres depuis appsettings.json directement (sans DI).
/// </summary>
public sealed class ScolariteDbContextFactory : IDesignTimeDbContextFactory<ScolariteDbContext>
{
    public ScolariteDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Chercher appsettings.json en remontant depuis le répertoire courant
        string? webAppDir = null;
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "ProjetScolariteSOLID"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "ProjetScolariteSOLID"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "ProjetScolariteSOLID"),
        };
        foreach (var c in candidates)
        {
            if (File.Exists(Path.Combine(c, "appsettings.json"))) { webAppDir = c; break; }
        }
        if (webAppDir is null)
            throw new InvalidOperationException("Impossible de localiser appsettings.json.");

        var appsettingsPath    = Path.Combine(webAppDir, "appsettings.json");
        var appsettingsEnvPath = Path.Combine(webAppDir, $"appsettings.{environmentName}.json");

        var config = new System.Collections.Generic.Dictionary<string, string>();

        // Lire le fichier appsettings.json de base
        if (File.Exists(appsettingsPath))
        {
            var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(appsettingsPath));
            if (json.RootElement.TryGetProperty("ConnectionStrings", out var connStrings))
            {
                if (connStrings.TryGetProperty("ScolariteDb", out var dbConn))
                {
                    config["ConnectionStrings:ScolariteDb"] = dbConn.GetString() ?? string.Empty;
                }
            }
        }

        // Lire le fichier appsettings.{environment}.json (overwrite)
        if (File.Exists(appsettingsEnvPath))
        {
            var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(appsettingsEnvPath));
            if (json.RootElement.TryGetProperty("ConnectionStrings", out var connStrings))
            {
                if (connStrings.TryGetProperty("ScolariteDb", out var dbConn))
                {
                    config["ConnectionStrings:ScolariteDb"] = dbConn.GetString() ?? string.Empty;
                }
            }
        }

        var connectionString = config.TryGetValue("ConnectionStrings:ScolariteDb", out var cs) 
            ? cs 
            : throw new InvalidOperationException("ConnectionString 'ScolariteDb' not found in appsettings.json");

        var optionsBuilder = new DbContextOptionsBuilder<ScolariteDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ScolariteDbContext(optionsBuilder.Options);
    }
}
