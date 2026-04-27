using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfAuditLogRepository : IAuditLogRepository
{
    private readonly ScolariteDbContext _db;

    public EfAuditLogRepository(ScolariteDbContext db) => _db = db;

    public async Task<AuditLog?> GetByIdAsync(long id, CancellationToken ct = default)
        => await _db.AuditLogs.FirstOrDefaultAsync(a => a.Id == id, ct);

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

    public async Task<(IReadOnlyList<AuditLog> Items, int Total)> GetPagedAsync(
        int skip, int take,
        string search, string? tableFilter, string? userFilter,
        int sortCol, string sortDir,
        CancellationToken ct = default)
    {
        var q = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(tableFilter))
            q = q.Where(a => a.TableName == tableFilter);

        if (!string.IsNullOrWhiteSpace(userFilter))
            q = q.Where(a => a.UserId != null && a.UserId.Contains(userFilter));

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(a => a.TableName.Contains(search)
                           || a.Action.Contains(search)
                           || a.KeyValues.Contains(search)
                           || (a.UserId != null && a.UserId.Contains(search)));

        var total = await q.CountAsync(ct);

        q = (sortCol, sortDir.ToLower()) switch
        {
            (1, "asc")  => q.OrderBy(a => a.Timestamp),
            (1, _)      => q.OrderByDescending(a => a.Timestamp),
            (2, "asc")  => q.OrderBy(a => a.TableName),
            (2, _)      => q.OrderByDescending(a => a.TableName),
            (3, "asc")  => q.OrderBy(a => a.Action),
            (3, _)      => q.OrderByDescending(a => a.Action),
            _           => q.OrderByDescending(a => a.Timestamp)
        };

        var items = await q.Skip(skip).Take(take).ToListAsync(ct);
        return (items, total);
    }

    public async Task<IReadOnlyList<string>> GetDistinctTablesAsync(CancellationToken ct = default)
        => await _db.AuditLogs
                    .Select(a => a.TableName)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToListAsync(ct);
}

