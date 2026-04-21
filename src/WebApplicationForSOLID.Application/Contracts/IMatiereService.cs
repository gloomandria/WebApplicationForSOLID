using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Application.Contracts;

public interface IMatiereService
{
    Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default);
    Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Matiere>> CreateAsync(Matiere matiere, CancellationToken ct = default);
    Task<OperationResult> UpdateAsync(Matiere matiere, CancellationToken ct = default);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);
}
