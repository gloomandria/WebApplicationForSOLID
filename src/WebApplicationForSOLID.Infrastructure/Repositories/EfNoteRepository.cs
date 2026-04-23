namespace ProjetScolariteSOLID.Infrastructure.Repositories;

public sealed class EfNoteRepository : INoteRepository
{
    private readonly ScolariteDbContext _context;

    public EfNoteRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Note?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Notes
                         .AsNoTracking()
                         .Include(n => n.Etudiant)
                         .Include(n => n.Matiere)
                         .FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct = default)
        => await _context.Notes
                         .AsNoTracking()
                         .Include(n => n.Etudiant)
                         .Include(n => n.Matiere)
                         .OrderByDescending(n => n.Date)
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<Note>> GetByEtudiantAsync(int etudiantId, CancellationToken ct = default)
        => await _context.Notes
                         .AsNoTracking()
                         .Include(n => n.Matiere)
                         .Where(n => n.EtudiantId == etudiantId)
                         .OrderByDescending(n => n.Date)
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<Note>> GetByMatiereAsync(int matiereId, CancellationToken ct = default)
        => await _context.Notes
                         .AsNoTracking()
                         .Include(n => n.Etudiant)
                         .Where(n => n.MatiereId == matiereId)
                         .OrderByDescending(n => n.Date)
                         .ToListAsync(ct);

    public async Task<IReadOnlyList<Note>> GetByEtudiantAndMatiereAsync(int etudiantId, int matiereId, CancellationToken ct = default)
        => await _context.Notes
                         .AsNoTracking()
                         .Where(n => n.EtudiantId == etudiantId && n.MatiereId == matiereId)
                         .OrderByDescending(n => n.Date)
                         .ToListAsync(ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _context.Notes.AnyAsync(n => n.Id == id, ct);

    public async Task<PagedResult<Note>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        IQueryable<Note> q = _context.Notes
                        .AsNoTracking()
                        .Include(n => n.Etudiant).ThenInclude(e => e!.User)
                        .Include(n => n.Matiere)
                        .Include(n => n.TypeEvaluation);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(n => (n.Etudiant != null && n.Etudiant.User != null
                              && (n.Etudiant.User.Nom.ToLower().Contains(s)
                               || n.Etudiant.User.Prenom.ToLower().Contains(s)))
                          || (n.Matiere != null && n.Matiere.Intitule.ToLower().Contains(s))
                          || (n.TypeEvaluation != null && n.TypeEvaluation.Libelle.ToLower().Contains(s))
                          || n.Commentaire.ToLower().Contains(s));
        }

        bool asc = sortDir != "desc";
        IOrderedQueryable<Note> ordered = sortCol switch
        {
            1 => asc ? q.OrderBy(n => n.Matiere!.Intitule)            : q.OrderByDescending(n => n.Matiere!.Intitule),
            2 => asc ? q.OrderBy(n => n.Valeur)                       : q.OrderByDescending(n => n.Valeur),
            3 => asc ? q.OrderBy(n => n.TypeEvaluation!.Libelle)      : q.OrderByDescending(n => n.TypeEvaluation!.Libelle),
            4 => asc ? q.OrderBy(n => n.Date)                         : q.OrderByDescending(n => n.Date),
            5 => asc ? q.OrderBy(n => n.Commentaire)                  : q.OrderByDescending(n => n.Commentaire),
            _ => asc ? q.OrderBy(n => n.Etudiant!.User!.Nom).ThenBy(n => n.Etudiant!.User!.Prenom)
                     : q.OrderByDescending(n => n.Etudiant!.User!.Nom).ThenByDescending(n => n.Etudiant!.User!.Prenom)
        };

        var totalCount = await ordered.CountAsync(ct);
        var items = await ordered
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);

        return new PagedResult<Note>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Note> AddAsync(Note entity, CancellationToken ct = default)
    {
        await _context.Notes.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(Note entity, CancellationToken ct = default)
    {
        _context.Notes.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Notes
                      .Where(n => n.Id == id)
                      .ExecuteDeleteAsync(ct);
    }
}
