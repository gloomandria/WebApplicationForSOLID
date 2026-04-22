using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjetScolariteSOLID.Infrastructure.Notifications;
using ProjetScolariteSOLID.Infrastructure.Repositories;

namespace ProjetScolariteSOLID.Infrastructure.Extensions;

/// <summary>
/// OCP — Centralise l'enregistrement de la couche Infrastructure.
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── SQL Server avec retry automatique ─────────────────────────────────────
        services.AddDbContext<ScolariteDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ScolariteDb"),
                sql => sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)));

        // ── Repositories EF Core (Scoped = même DbContext par requête) ────────────
        services.AddScoped<IEtudiantRepository,    EfEtudiantRepository>();
        services.AddScoped<IEnseignantRepository,  EfEnseignantRepository>();
        services.AddScoped<IMatiereRepository,     EfMatiereRepository>();
        services.AddScoped<IClasseRepository,      EfClasseRepository>();
        services.AddScoped<IInscriptionRepository, EfInscriptionRepository>();
        services.AddScoped<INoteRepository,        EfNoteRepository>();

        // ── Notification — log console + persistance BDD ──────────────────────────
        services.AddScoped<INotificationService, DatabaseNotificationService>();

        // ── Seed ──────────────────────────────────────────────────────────────────
        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
