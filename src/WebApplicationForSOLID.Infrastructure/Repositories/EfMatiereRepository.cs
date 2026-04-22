using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfMatiereRepository : IMatiereRepository
{
    private readonly ScolariteDbContext _context;

    public EfMatiereRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Matiere?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Matieres
                         .AsNoTracking()
                         .Include(m => m.Enseignant)
                         .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IReadOnlyList<Matiere>> GetAllAsync(CancellationToken ct = default)
        => await _context.Matieres
                         .AsNoTracking()
                         .OrderBy(m => m.Intitule)
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<Matiere>> GetAllWithEnseignantAsync(CancellationToken ct = default)
        => await _context.Matieres
                         .AsNoTracking()
                         .Include(m => m.Enseignant)
                         .OrderBy(m => m.Intitule)
                         .ToListAsync(ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _context.Matieres.AnyAsync(m => m.Id == id, ct);

    public Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default)
        => _context.Matieres.AnyAsync(
               m => m.Code == code && m.Id != excludeId, ct);

    public async Task<Matiere> AddAsync(Matiere entity, CancellationToken ct = default)
    {
        await _context.Matieres.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Matiere entity, CancellationToken ct = default)
    {
        _context.Matieres.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Matieres
                      .Where(m => m.Id == id)
                      .ExecuteDeleteAsync(ct);
    }
}
