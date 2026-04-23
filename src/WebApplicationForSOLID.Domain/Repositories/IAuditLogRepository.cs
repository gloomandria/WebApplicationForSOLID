using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetByTableAsync(string tableName, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetByUserAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetRecentAsync(int count = 100, CancellationToken ct = default);

    // ── DataTable server-side ─────────────────────────────────────────────────
    Task<(IReadOnlyList<AuditLog> Items, int Total)> GetPagedAsync(
        int skip, int take,
        string search, string? tableFilter, string? userFilter,
        int sortCol, string sortDir,
        CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetDistinctTablesAsync(CancellationToken ct = default);
}
