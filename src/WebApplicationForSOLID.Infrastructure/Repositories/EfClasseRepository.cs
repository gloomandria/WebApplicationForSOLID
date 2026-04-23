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

    public async Task<PagedResult<Classe>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        IQueryable<Classe> q = _context.Classes
                        .AsNoTracking()
                        .Include(c => c.Niveau)
                        .Include(c => c.Filiere)
                        .Include(c => c.AnneeAcademique);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(c => c.Nom.ToLower().Contains(s)
                          || (c.Niveau != null && c.Niveau.Libelle.ToLower().Contains(s))
                          || (c.Filiere != null && c.Filiere.Libelle.ToLower().Contains(s))
                          || (c.AnneeAcademique != null && c.AnneeAcademique.Libelle.ToLower().Contains(s)));
        }

        bool asc = sortDir != "desc";
        IOrderedQueryable<Classe> ordered = sortCol switch
        {
            1 => asc ? q.OrderBy(c => c.Niveau!.Libelle)          : q.OrderByDescending(c => c.Niveau!.Libelle),
            2 => asc ? q.OrderBy(c => c.Filiere!.Libelle)         : q.OrderByDescending(c => c.Filiere!.Libelle),
            3 => asc ? q.OrderBy(c => c.AnneeAcademique!.Libelle) : q.OrderByDescending(c => c.AnneeAcademique!.Libelle),
            4 => asc ? q.OrderBy(c => c.CapaciteMax)              : q.OrderByDescending(c => c.CapaciteMax),
            _ => asc ? q.OrderBy(c => c.Nom)                      : q.OrderByDescending(c => c.Nom)
        };

        var totalCount = await ordered.CountAsync(ct);
        var items = await ordered
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);

        return new PagedResult<Classe>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<int> GetNombreEtudiantsAsync(int classeId, CancellationToken ct = default)
        => _context.Inscriptions
                   .CountAsync(i => i.ClasseId == classeId
                               && i.Statut != null && i.Statut.Libelle == "Active", ct);

    public async Task<IReadOnlyList<(int ClasseId, string ClasseNom, double? Moyenne)>> GetMoyennesParClasseAsync(CancellationToken ct = default)
    {
        var result = await _context.Classes
            .AsNoTracking()
            .OrderBy(c => c.Nom)
            .Select(c => new
            {
                c.Id,
                c.Nom,
                Moyenne = _context.Notes
                    .Where(n => _context.Inscriptions
                        .Any(i => i.ClasseId == c.Id && i.EtudiantId == n.EtudiantId
                              && i.Statut != null && i.Statut.Libelle == "Active"))
                    .Average(n => (double?)n.Valeur)
            })
            .ToListAsync(ct);

        return result.Select(r => (r.Id, r.Nom, r.Moyenne)).ToList();
    }

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
