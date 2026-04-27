namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfEnseignantRepository : IEnseignantRepository
{
    private readonly ScolariteDbContext _context;

    public EfEnseignantRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Enseignant?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Enseignants
                         .Include(e => e.User)
                         .Include(e => e.Specialite)
                         .Include(e => e.Grade)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Enseignant>> GetAllAsync(CancellationToken ct = default)
        => await _context.Enseignants
                         .Include(e => e.User)
                         .Include(e => e.Specialite)
                         .Include(e => e.Grade)
                         .AsNoTracking()
                         .OrderBy(e => e.User!.Nom).ThenBy(e => e.User!.Prenom)
                         .ToListAsync(ct);

    public async Task<PagedResult<Enseignant>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        IQueryable<Enseignant> q = _context.Enseignants
                        .Include(e => e.User)
                        .Include(e => e.Specialite)
                        .Include(e => e.Grade)
                        .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(e => (e.Matricule != null && e.Matricule.ToLower().Contains(s))
                          || e.User!.Nom.ToLower().Contains(s)
                          || e.User!.Prenom.ToLower().Contains(s)
                          || (e.User!.Email != null && e.User.Email.ToLower().Contains(s))
                          || (e.Specialite != null && e.Specialite.Libelle.ToLower().Contains(s))
                          || (e.Grade != null && e.Grade.Libelle.ToLower().Contains(s)));
        }

        bool asc = sortDir != "desc";
        IOrderedQueryable<Enseignant> ordered = sortCol switch
        {
            0 => asc ? q.OrderBy(e => e.Matricule)             : q.OrderByDescending(e => e.Matricule),
            2 => asc ? q.OrderBy(e => e.User!.Email)           : q.OrderByDescending(e => e.User!.Email),
            3 => asc ? q.OrderBy(e => e.Specialite!.Libelle)   : q.OrderByDescending(e => e.Specialite!.Libelle),
            4 => asc ? q.OrderBy(e => e.Grade!.Libelle)        : q.OrderByDescending(e => e.Grade!.Libelle),
            _ => asc ? q.OrderBy(e => e.User!.Nom).ThenBy(e => e.User!.Prenom)
                     : q.OrderByDescending(e => e.User!.Nom).ThenByDescending(e => e.User!.Prenom)
        };

        var totalCount = await ordered.CountAsync(ct);
        var items = await ordered
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
        // Détache la navigation User pour éviter les conflits de tracking avec Identity
        entity.User = null;

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
