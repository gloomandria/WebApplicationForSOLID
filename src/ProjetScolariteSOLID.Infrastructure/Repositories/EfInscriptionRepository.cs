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
                         .Include(i => i.Statut)
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
                    i.ClasseId == classeId, ct);

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

    public async Task<PagedResult<Inscription>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        IQueryable<Inscription> q = _context.Inscriptions
                        .AsNoTracking()
                        .Include(i => i.Etudiant).ThenInclude(e => e!.User)
                        .Include(i => i.Classe)
                        .Include(i => i.Statut);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(i => (i.Etudiant != null && i.Etudiant.User != null
                              && (i.Etudiant.User.Nom.ToLower().Contains(s)
                               || i.Etudiant.User.Prenom.ToLower().Contains(s)))
                          || (i.Classe != null && i.Classe.Nom.ToLower().Contains(s))
                          || (i.Statut != null && i.Statut.Libelle.ToLower().Contains(s)));
        }

        bool asc = sortDir != "desc";
        IOrderedQueryable<Inscription> ordered = sortCol switch
        {
            1 => asc ? q.OrderBy(i => i.Classe!.Nom)            : q.OrderByDescending(i => i.Classe!.Nom),
            2 => asc ? q.OrderBy(i => i.DateInscription)        : q.OrderByDescending(i => i.DateInscription),
            3 => asc ? q.OrderBy(i => i.Statut!.Libelle)        : q.OrderByDescending(i => i.Statut!.Libelle),
            _ => asc ? q.OrderBy(i => i.Etudiant!.User!.Nom).ThenBy(i => i.Etudiant!.User!.Prenom)
                     : q.OrderByDescending(i => i.Etudiant!.User!.Nom).ThenByDescending(i => i.Etudiant!.User!.Prenom)
        };

        var totalCount = await ordered.CountAsync(ct);
        var items = await ordered
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
