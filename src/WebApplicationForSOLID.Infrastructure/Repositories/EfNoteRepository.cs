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

    public async Task<PagedResult<Note>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Notes
                            .AsNoTracking()
                            .Include(n => n.Etudiant)
                            .Include(n => n.Matiere)
                            .OrderByDescending(n => n.Date);

        var totalCount = await query.CountAsync(ct);
        var items = await query
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
