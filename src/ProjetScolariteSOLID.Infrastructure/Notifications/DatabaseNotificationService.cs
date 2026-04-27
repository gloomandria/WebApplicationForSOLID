namespace ProjetScolariteSOLID.Infrastructure.Notifications;

/// <summary>
/// Implémentation INotificationService qui persiste les notifications via Serilog (sink SQL Server).
/// LSP — substitue INotificationService. SRP — responsabilité unique : notifier et tracer en BDD.
/// </summary>
public sealed class DatabaseNotificationService : INotificationService
{
    private readonly ILogger<DatabaseNotificationService> _logger;

    public DatabaseNotificationService(ILogger<DatabaseNotificationService> logger)
        => _logger = logger;

    public Task NotifyInscriptionAsync(
        int etudiantId, string nomEtudiant, string nomClasse, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[NOTIFICATION] Nouvelle inscription — Étudiant: {NomEtudiant} (Id={EtudiantId}) → Classe: {NomClasse}",
            nomEtudiant, etudiantId, nomClasse);
        return Task.CompletedTask;
    }

    public Task NotifyNoteAjouteeAsync(
        int etudiantId, string nomEtudiant, string matiere, decimal note, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[NOTIFICATION] Note ajoutée — Étudiant: {NomEtudiant} (Id={EtudiantId}), Matière: {Matiere}, Note: {Note}/20",
            nomEtudiant, etudiantId, matiere, note);
        return Task.CompletedTask;
    }
}
