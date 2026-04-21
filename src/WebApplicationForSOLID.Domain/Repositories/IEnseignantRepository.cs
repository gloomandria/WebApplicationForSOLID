using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Domain.Repositories;

public interface IEnseignantRepository : IReadRepository<Enseignant>, IWriteRepository<Enseignant>
{
    Task<PagedResult<Enseignant>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);
}
