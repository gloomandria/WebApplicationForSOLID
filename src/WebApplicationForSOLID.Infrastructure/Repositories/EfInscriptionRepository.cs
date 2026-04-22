using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.Infrastructure.Data;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfInscriptionRepository : IInscriptionRepository
{
    private readonly ScolariteDbContext _context;

    public EfInscriptionRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Inscription?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Inscriptions
                         .AsNoTracking()
                         .Include(i => i.Etudiant)
                         .Include(i => i.Classe)
                         .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IReadOnlyList<Inscription>> GetAllAsync(CancellationToken ct = default)
        => await _context.Inscriptions
                         .AsNoTracking()
                         .Include(i => i.Etudiant)
                         .Include(i => i.Classe)
                         .OrderByDescending(i => i.DateInscription)
                         .ToListAsync(ct);

    public Task<bool> ExistsAsync(int etudiantId, int classeId, CancellationToken ct = default)
        => _context.Inscriptions.AnyAsync(
               i => i.EtudiantId == etudiantId &&
                    i.ClasseId == classeId &&
                    i.Statut == StatutInscription.Active, ct);

    public async Task<IReadOnlyList<Inscription>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default)
        => await _context.Inscriptions
                         .AsNoTracking()
                         .Include(i => i.Classe)
                         .Where(i => i.EtudiantId == etudiantId)
                         .OrderByDescending(i => i.DateInscription)
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<Inscription>> GetByClasseAsync(int classeId, CancellationToken ct = default)
        => await _context.Inscriptions
                         .AsNoTracking()
                         .Include(i => i.Etudiant)
                         .Where(i => i.ClasseId == classeId)
                         .ToListAsync(ct);

    public async Task<PagedResult<Inscription>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Inscriptions
                            .AsNoTracking()
                            .Include(i => i.Etudiant)
                            .Include(i => i.Classe)
                            .OrderByDescending(i => i.DateInscription);

        var totalCount = await query.CountAsync(ct);
        var items = await query
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);

        return new PagedResult<Inscription>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Inscription> AddAsync(Inscription entity, CancellationToken ct = default)
    {
        await _context.Inscriptions.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Inscription entity, CancellationToken ct = default)
    {
        _context.Inscriptions.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Inscriptions
                      .Where(i => i.Id == id)
                      .ExecuteDeleteAsync(ct);
    }
}
