using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

/// <summary>
/// Implémentation générique EF Core pour les référentiels simples (lecture + écriture).
/// </summary>
public sealed class EfReferentielRepository<T> : IReferentielRepository<T> where T : class
{
    private readonly ScolariteDbContext _context;

    public EfReferentielRepository(ScolariteDbContext context) => _context = context;

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => _context.Set<T>().AsNoTracking().ToListAsync(ct)
                   .ContinueWith(t => (IReadOnlyList<T>)t.Result, ct);

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Set<T>().FindAsync([id], ct).AsTask()!;

    public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Set<T>().FindAsync([id], ct);
        if (entity is not null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
