using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfClasseRepository : IClasseRepository
{
    private readonly ScolariteDbContext _context;

    public EfClasseRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Classe?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Classes
                         .AsNoTracking()
                         .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Classe>> GetAllAsync(CancellationToken ct = default)
        => await _context.Classes
                         .AsNoTracking()
                         .OrderBy(c => c.Nom)
                         .ToListAsync(ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _context.Classes.AnyAsync(c => c.Id == id, ct);

    public Task<int> GetNombreEtudiantsAsync(int classeId, CancellationToken ct = default)
        => _context.Inscriptions
                   .CountAsync(i => i.ClasseId == classeId && i.Statut == StatutInscription.Active, ct);

    public async Task<Classe> AddAsync(Classe entity, CancellationToken ct = default)
    {
        await _context.Classes.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Classe entity, CancellationToken ct = default)
    {
        _context.Classes.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Classes
                      .Where(c => c.Id == id)
                      .ExecuteDeleteAsync(ct);
    }
}
