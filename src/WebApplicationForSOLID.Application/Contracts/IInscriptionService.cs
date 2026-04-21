using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Contracts;

public interface IInscriptionService
{
    Task<PagedResult<Inscription>> GetInscriptionsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<OperationResult<Inscription>> InscrireEtudiantAsync(int etudiantId, int classeId, CancellationToken ct = default);
    Task<OperationResult> ModifierStatutAsync(int inscriptionId, StatutInscription statut, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetByClasseAsync(int classeId, CancellationToken ct = default);
}
