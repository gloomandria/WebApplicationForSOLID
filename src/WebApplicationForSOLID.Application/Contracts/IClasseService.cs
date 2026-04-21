using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Contracts;

public interface IClasseService
{
    Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default);
    Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Classe>> CreateAsync(Classe classe, CancellationToken ct = default);
    Task<OperationResult> UpdateAsync(Classe classe, CancellationToken ct = default);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);
}
