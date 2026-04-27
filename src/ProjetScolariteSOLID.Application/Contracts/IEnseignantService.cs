using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Contracts;

public interface IEnseignantService
{
    Task<PagedResult<Enseignant>> GetEnseignantsAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default);
    Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Enseignant>> CreateAsync(Enseignant enseignant, CancellationToken ct = default);
    Task<OperationResult> UpdateAsync(Enseignant enseignant, CancellationToken ct = default);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default);
}
