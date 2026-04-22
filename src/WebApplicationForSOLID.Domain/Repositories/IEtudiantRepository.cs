using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

/// <summary>
/// OCP — Extension des interfaces génériques avec des requêtes spécifiques aux étudiants.
/// </summary>
public interface IEtudiantRepository : IReadRepository<Etudiant>, IWriteRepository<Etudiant>
{
    Task<PagedResult<Etudiant>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Etudiant>> GetByClasseAsync(int classeId, CancellationToken ct = default);
}
