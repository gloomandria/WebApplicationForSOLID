using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Auth;
using ProjetScolariteSOLID.Infrastructure.Email;
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

        // ── ASP.NET Core Identity ─────────────────────────────────────────────────
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength         = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail         = true;
            options.SignIn.RequireConfirmedEmail     = true;
        })
        .AddEntityFrameworkStores<ScolariteDbContext>()
        .AddDefaultTokenProviders();

        // ── Repositories EF Core (Scoped = même DbContext par requête) ────────────
        services.AddScoped<IEtudiantRepository,    EfEtudiantRepository>();
        services.AddScoped<IEnseignantRepository,  EfEnseignantRepository>();
        services.AddScoped<IMatiereRepository,     EfMatiereRepository>();
        services.AddScoped<IClasseRepository,      EfClasseRepository>();
        services.AddScoped<IInscriptionRepository, EfInscriptionRepository>();
        services.AddScoped<INoteRepository,        EfNoteRepository>();

        // ── Référentiels ──────────────────────────────────────────────────────────
        services.AddScoped<IReferentielRepository<Filiere>,              EfReferentielRepository<Filiere>>();
        services.AddScoped<IReferentielRepository<AnneeAcademique>,      EfReferentielRepository<AnneeAcademique>>();
        services.AddScoped<IReferentielRepository<Niveau>,               EfReferentielRepository<Niveau>>();
        services.AddScoped<IReferentielRepository<Specialite>,           EfReferentielRepository<Specialite>>();
        services.AddScoped<IReferentielRepository<Grade>,                EfReferentielRepository<Grade>>();
        services.AddScoped<IReferentielRepository<StatutInscriptionRef>, EfReferentielRepository<StatutInscriptionRef>>();
        services.AddScoped<IReferentielRepository<TypeEvaluationRef>,    EfReferentielRepository<TypeEvaluationRef>>();

        // ── Notification — log console + persistance BDD ──────────────────────────
        services.AddScoped<INotificationService, DatabaseNotificationService>();

        // ── Auth / Permissions ────────────────────────────────────────────────────
        services.AddScoped<IPermissionService, PermissionService>();

        // ── Email Queue ───────────────────────────────────────────────────────────
        services.AddScoped<IEmailQueueService, EfEmailQueueService>();
        services.AddScoped<ISmtpEmailSender,   SmtpEmailSender>();
        services.AddHostedService<EmailQueueBackgroundService>();

        // ── Seed ──────────────────────────────────────────────────────────────────
        services.AddScoped<IDataSeeder, DataSeeder>();

        // ── Audit ─────────────────────────────────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<IAuditLogRepository, EfAuditLogRepository>();

        return services;
    }
}
