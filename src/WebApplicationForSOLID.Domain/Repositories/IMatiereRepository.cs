using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Domain.Repositories;

public interface IMatiereRepository : IReadRepository<Matiere>, IWriteRepository<Matiere>
{
    Task<IReadOnlyList<Matiere>> GetAllWithEnseignantAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default);
}
