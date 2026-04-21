using Microsoft.Extensions.DependencyInjection;
using WebApplicationForSOLID.Application.Contracts;
using WebApplicationForSOLID.Application.CQRS.Behaviors;
using WebApplicationForSOLID.Application.Services;
using WebApplicationForSOLID.Application.Validators;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Extensions;

/// <summary>
/// OCP — Centralise l'enregistrement de la couche Application.
/// Web appelle uniquement cette méthode, sans connaître les implémentations.
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ── MediatR — découverte automatique de tous les Handlers de cet assembly ──
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

            // Pipeline : Logging → Validation → Handler
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // ── Validateurs ───────────────────────────────────────────────────────────
        services.AddSingleton<IValidator<Etudiant>,   EtudiantValidator>();
        services.AddSingleton<IValidator<Enseignant>, EnseignantValidator>();
        services.AddSingleton<IValidator<Matiere>,    MatiereValidator>();
        services.AddSingleton<IValidator<Note>,       NoteValidator>();

        // ── Services métier (Scoped = une instance par requête HTTP) ──────────────
        services.AddScoped<IEtudiantService,    EtudiantService>();
        services.AddScoped<IEnseignantService,  EnseignantService>();
        services.AddScoped<IMatiereService,     MatiereService>();
        services.AddScoped<IClasseService,      ClasseService>();
        services.AddScoped<IInscriptionService, InscriptionService>();
        services.AddScoped<INoteService,        NoteService>();

        return services;
    }
}
