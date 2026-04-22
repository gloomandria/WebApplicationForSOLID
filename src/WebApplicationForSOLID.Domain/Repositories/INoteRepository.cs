using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

public interface INoteRepository : IReadRepository<Note>, IWriteRepository<Note>
{
    Task<IReadOnlyList<Note>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetByMatiereAsync(int matiereId, CancellationToken ct = default);
    Task<IReadOnlyList<Note>> GetByEtudiantAndMatiereAsync(int etudiantId, int matiereId, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<PagedResult<Note>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
}
