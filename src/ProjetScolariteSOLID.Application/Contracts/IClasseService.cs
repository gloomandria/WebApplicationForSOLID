using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Contracts;

public interface IClasseService
{
    Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default);
    Task<PagedResult<Classe>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default);
    Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Classe>> CreateAsync(Classe classe, CancellationToken ct = default);
    Task<OperationResult> UpdateAsync(Classe classe, CancellationToken ct = default);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);
}
