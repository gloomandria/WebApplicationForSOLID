using Microsoft.EntityFrameworkCore;
using WebApplicationForSOLID.Application.Extensions;
using WebApplicationForSOLID.Infrastructure.Data;
using WebApplicationForSOLID.Infrastructure.Extensions;
using WebApplicationForSOLID.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddApplicationServices();                          // Application : MediatR + Services + Validators
builder.Services.AddInfrastructureServices(builder.Configuration); // Infrastructure : DbContext + Repositories

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app);

app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();

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

        if (app.Environment.IsDevelopment())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
            await seeder.SeedAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur lors de la migration ou du seed.");
        throw;
    }
}
