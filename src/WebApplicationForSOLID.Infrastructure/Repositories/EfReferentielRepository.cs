using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

/// <summary>
/// Implémentation générique EF Core pour les référentiels simples (lecture seule).
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
}
