using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Domain.Repositories;

public interface IClasseRepository : IReadRepository<Classe>, IWriteRepository<Classe>
{
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<int> GetNombreEtudiantsAsync(int classeId, CancellationToken ct = default);
}
