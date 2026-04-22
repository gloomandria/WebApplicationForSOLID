using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfAuditLogRepository : IAuditLogRepository
{
    private readonly ScolariteDbContext _db;

    public EfAuditLogRepository(ScolariteDbContext db) => _db = db;

    public async Task<IReadOnlyList<AuditLog>> GetByTableAsync(string tableName, CancellationToken ct = default)
        => await _db.AuditLogs
                    .Where(a => a.TableName == tableName)
                    .OrderByDescending(a => a.Timestamp)
                    .ToListAsync(ct);

    public async Task<IReadOnlyList<AuditLog>> GetByUserAsync(string userId, CancellationToken ct = default)
        => await _db.AuditLogs
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.Timestamp)
                    .ToListAsync(ct);

    public async Task<IReadOnlyList<AuditLog>> GetRecentAsync(int count = 100, CancellationToken ct = default)
        => await _db.AuditLogs
                    .OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .ToListAsync(ct);
}
