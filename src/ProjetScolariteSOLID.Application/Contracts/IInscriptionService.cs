using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Contracts;

public interface IInscriptionService
{
    Task<PagedResult<Inscription>> GetInscriptionsAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default);
    Task<Inscription?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Inscription>> InscrireEtudiantAsync(int etudiantId, int classeId, CancellationToken ct = default);
    Task<OperationResult> ModifierStatutAsync(int inscriptionId, int statutId, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetByClasseAsync(int classeId, CancellationToken ct = default);
    Task<OperationResult> SupprimerAsync(int id, CancellationToken ct = default);
}
