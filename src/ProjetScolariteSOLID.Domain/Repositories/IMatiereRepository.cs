using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

public interface IMatiereRepository : IReadRepository<Matiere>, IWriteRepository<Matiere>
{
    Task<IReadOnlyList<Matiere>> GetAllWithEnseignantAsync(CancellationToken ct = default);
    Task<PagedResult<Matiere>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default);
}
