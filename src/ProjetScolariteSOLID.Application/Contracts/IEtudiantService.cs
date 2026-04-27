using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Application.Contracts;

/// <summary>
/// DIP — Les PageModel dépendent de cette abstraction.
/// </summary>
public interface IEtudiantService
{
    Task<PagedResult<Etudiant>> GetEtudiantsAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default);
    Task<Etudiant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OperationResult<Etudiant>> CreateAsync(Etudiant etudiant, CancellationToken ct = default);
    Task<OperationResult> UpdateAsync(Etudiant etudiant, CancellationToken ct = default);
    Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Etudiant>> GetAllAsync(CancellationToken ct = default);
}
