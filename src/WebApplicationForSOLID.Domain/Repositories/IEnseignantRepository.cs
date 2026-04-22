using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

public interface IEnseignantRepository : IReadRepository<Enseignant>, IWriteRepository<Enseignant>
{
    Task<PagedResult<Enseignant>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
