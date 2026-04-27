using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

public interface IInscriptionRepository : IReadRepository<Inscription>, IWriteRepository<Inscription>
{
    Task<bool> ExistsAsync(int etudiantId, int classeId, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default);
    Task<IReadOnlyList<Inscription>> GetByClasseAsync(int classeId, CancellationToken ct = default);
    Task<PagedResult<Inscription>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
}
