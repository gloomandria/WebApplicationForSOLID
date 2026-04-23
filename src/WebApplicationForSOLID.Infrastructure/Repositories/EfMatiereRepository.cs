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

    public async Task<PagedResult<Matiere>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        IQueryable<Matiere> q = _context.Matieres
                        .AsNoTracking()
                        .Include(m => m.Enseignant)
                        .ThenInclude(e => e!.User);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(m => m.Code.ToLower().Contains(s)
                          || m.Intitule.ToLower().Contains(s)
                          || (m.Enseignant != null && m.Enseignant.User != null
                              && (m.Enseignant.User.Nom.ToLower().Contains(s)
                               || m.Enseignant.User.Prenom.ToLower().Contains(s))));
        }

        bool asc = sortDir != "desc";
        IOrderedQueryable<Matiere> ordered = sortCol switch
        {
            0 => asc ? q.OrderBy(m => m.Code)          : q.OrderByDescending(m => m.Code),
            2 => asc ? q.OrderBy(m => m.Coefficient)   : q.OrderByDescending(m => m.Coefficient),
            3 => asc ? q.OrderBy(m => m.VolumeHoraire) : q.OrderByDescending(m => m.VolumeHoraire),
            4 => asc ? q.OrderBy(m => m.Enseignant!.User!.Nom) : q.OrderByDescending(m => m.Enseignant!.User!.Nom),
            _ => asc ? q.OrderBy(m => m.Intitule)      : q.OrderByDescending(m => m.Intitule)
        };

        var totalCount = await ordered.CountAsync(ct);
        var items = await ordered
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);

        return new PagedResult<Matiere>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

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
