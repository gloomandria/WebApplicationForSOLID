using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> GetByTableAsync(string tableName, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetByUserAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetRecentAsync(int count = 100, CancellationToken ct = default);
}
