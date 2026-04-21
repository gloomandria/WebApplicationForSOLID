using Microsoft.EntityFrameworkCore;
using WebApplicationForSOLID.Domain.Repositories;
using WebApplicationForSOLID.Infrastructure.Data;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Infrastructure.Repositories;

public sealed class EfEnseignantRepository : IEnseignantRepository
{
    private readonly ScolariteDbContext _context;

    public EfEnseignantRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Enseignants
                         .AsNoTracking()
                         .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default)
        => await _context.Enseignants
                         .AsNoTracking()
                         .OrderBy(e => e.Nom).ThenBy(e => e.Prenom)
                         .ToListAsync(ct);

    public async Task<PagedResult<Enseignant>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Enseignants
                            .AsNoTracking()
                            .OrderBy(e => e.Nom).ThenBy(e => e.Prenom);

        var totalCount = await query.CountAsync(ct);
        var items = await query
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);

        return new PagedResult<Enseignant>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _context.Enseignants.AnyAsync(e => e.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => _context.Enseignants.AnyAsync(
               e => e.Email == email && e.Id != excludeId, ct);

    public async Task<Enseignant> AddAsync(Enseignant entity, CancellationToken ct = default)
    {
        await _context.Enseignants.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        // Matricule généré après obtention de l'Id
        entity.Matricule = $"ENS{entity.Id:D4}";
        await _context.SaveChangesAsync(ct);

        return entity;
    }

    public async Task UpdateAsync(Enseignant entity, CancellationToken ct = default)
    {
        _context.Enseignants.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Enseignants
                      .Where(e => e.Id == id)
                      .ExecuteDeleteAsync(ct);
    }
}
