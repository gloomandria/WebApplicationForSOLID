using WebApplicationForSOLID.Domain.Repositories;

namespace WebApplicationForSOLID.Infrastructure.Notifications;

/// <summary>
/// LSP  — Substitue INotificationService. Remplaçable par EmailNotificationService sans impact.
/// SRP  — Responsabilité unique : notifier via les logs.
/// </summary>
public sealed class LogNotificationService : INotificationService
{
    private readonly ILogger<LogNotificationService> _logger;

    public LogNotificationService(ILogger<LogNotificationService> logger)
        => _logger = logger;

    public Task NotifyInscriptionAsync(int etudiantId, string nomEtudiant, string nomClasse, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[NOTIFICATION] Nouvelle inscription — Étudiant: {Nom} (Id={Id}) → Classe: {Classe}",
            nomEtudiant, etudiantId, nomClasse);
        return Task.CompletedTask;
    }

    public Task NotifyNoteAjouteeAsync(int etudiantId, string nomEtudiant, string matiere, decimal note, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[NOTIFICATION] Note ajoutée — Étudiant: {Nom} (Id={Id}), Matière: {Matiere}, Note: {Note}/20",
            nomEtudiant, etudiantId, matiere, note);
        return Task.CompletedTask;
    }
}
