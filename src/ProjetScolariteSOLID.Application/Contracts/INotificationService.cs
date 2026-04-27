namespace ProjetScolariteSOLID.Application.Contracts;

/// <summary>
/// ISP — Séparé des services métier (SRP : notification = responsabilité unique).
/// </summary>
public interface INotificationService
{
    Task NotifyInscriptionAsync(int etudiantId, string nomEtudiant, string nomClasse, CancellationToken ct = default);
    Task NotifyNoteAjouteeAsync(int etudiantId, string nomEtudiant, string matiere, decimal note, CancellationToken ct = default);
}
