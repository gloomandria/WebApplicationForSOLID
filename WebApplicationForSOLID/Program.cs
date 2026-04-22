using Microsoft.EntityFrameworkCore;
using Serilog;
using ProjetScolariteSOLID.Application.Extensions;
using ProjetScolariteSOLID.Infrastructure.Data;
using ProjetScolariteSOLID.Infrastructure.Extensions;
using ProjetScolariteSOLID.Web.Middleware;

// Bootstrap logger pour capturer les erreurs au démarrage avant la configuration complète
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Remplacer le logging ASP.NET Core par Serilog configuré depuis appsettings.json
builder.Host.UseSerilog((ctx, services, cfg) =>
{
    try
    {
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .Enrich.WithMachineName()
           .Enrich.WithEnvironmentName();
    }
    catch (Exception ex)
    {
        // Si le sink SQL échoue (DB inaccessible), fallback console uniquement
        cfg.WriteTo.Console()
           .Enrich.FromLogContext()
           .Enrich.WithMachineName()
           .Enrich.WithEnvironmentName();
        Console.Error.WriteLine($"Serilog SQL sink failed, falling back to console: {ex.Message}");
    }
});

builder.Services.AddRazorPages();
builder.Services.AddApplicationServices();                          // Application : MediatR + Services + Validators
builder.Services.AddInfrastructureServices(builder.Configuration); // Infrastructure : DbContext + Repositories

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app);

// Mode seed-only : migrations + seed sans démarrer le serveur web
if (args.Contains("--seed-only"))
{
    Log.Information("Mode --seed-only : migrations et seed terminés. Arrêt.");
    Log.CloseAndFlush();
    return;
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redirection HTTPS uniquement si le serveur écoute sur HTTPS
var httpsPort = app.Configuration.GetValue<int?>("HTTPS_PORT") ?? app.Configuration.GetValue<int?>("ANCM_HTTPS_PORT");
if (httpsPort.HasValue)
{
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();

Log.CloseAndFlush();

static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ScolariteDbContext>();
        logger.LogInformation("Application des migrations...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations appliquées.");

        // Seed en Development et Release (idempotent, ne réinsère pas si données existent)
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur lors de la migration ou du seed. L'application démarre sans migration.");
    }
}
