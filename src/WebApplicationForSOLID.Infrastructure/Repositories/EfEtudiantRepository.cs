namespace ProjetScolariteSOLID.Infrastructure.Repositories;

/// <summary>
/// LSP  — Substitue parfaitement IEtudiantRepository (aucun contrat rompu).
/// SRP  — Responsabilité unique : accès données Etudiant via EF Core.
/// OCP  — Remplace InMemoryEtudiantRepository sans modifier aucun service.
/// </summary>
public sealed class EfEtudiantRepository : IEtudiantRepository
{
    private readonly ScolariteDbContext _context;

    public EfEtudiantRepository(ScolariteDbContext context)
        => _context = context;

    public async Task<Etudiant?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Etudiants
                         .Include(e => e.User)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Etudiant>> GetAllAsync(CancellationToken ct = default)
        => await _context.Etudiants
                         .Include(e => e.User)
                         .AsNoTracking()
                         .OrderBy(e => e.User!.Nom).ThenBy(e => e.User!.Prenom)
                         .ToListAsync(ct);

    public async Task<PagedResult<Etudiant>> GetPagedAsync(int page, int pageSize, string search = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        IQueryable<Etudiant> q = _context.Etudiants
                        .Include(e => e.User)
                        .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(e => (e.NumeroEtudiant != null && e.NumeroEtudiant.ToLower().Contains(s))
                          || (e.User!.Nom.ToLower().Contains(s))
                          || (e.User!.Prenom.ToLower().Contains(s))
                          || (e.User!.Email != null && e.User.Email.ToLower().Contains(s))
                          || (e.User!.PhoneNumber != null && e.User.PhoneNumber.ToLower().Contains(s)));
        }

        bool asc = sortDir != "desc";
        IOrderedQueryable<Etudiant> ordered = sortCol switch
        {
            0 => asc ? q.OrderBy(e => e.NumeroEtudiant)    : q.OrderByDescending(e => e.NumeroEtudiant),
            2 => asc ? q.OrderBy(e => e.User!.Email)       : q.OrderByDescending(e => e.User!.Email),
            3 => asc ? q.OrderBy(e => e.User!.PhoneNumber) : q.OrderByDescending(e => e.User!.PhoneNumber),
            4 => asc ? q.OrderBy(e => e.DateNaissance)     : q.OrderByDescending(e => e.DateNaissance),
            _ => asc ? q.OrderBy(e => e.User!.Nom).ThenBy(e => e.User!.Prenom)
                     : q.OrderByDescending(e => e.User!.Nom).ThenByDescending(e => e.User!.Prenom)
        };

        var totalCount = await ordered.CountAsync(ct);
        var items = await ordered
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);

        return new PagedResult<Etudiant>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _context.Etudiants.AnyAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Etudiant>> GetByClasseAsync(int classeId, CancellationToken ct = default)
        => await _context.Inscriptions
                         .AsNoTracking()
                         .Where(i => i.ClasseId == classeId
                                && i.Statut != null && i.Statut.Libelle == "Active")
                         .Include(i => i.Etudiant!.User)
                         .Select(i => i.Etudiant!)
                         .OrderBy(e => e.User!.Nom)
                         .ToListAsync(ct);

    public async Task<Etudiant> AddAsync(Etudiant entity, CancellationToken ct = default)
    {
        await _context.Etudiants.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        // Le NumeroEtudiant est généré après obtention de l'Id
        entity.NumeroEtudiant = $"ETU{entity.Id:D4}";
        await _context.SaveChangesAsync(ct);

        return entity;
    }

    public async Task UpdateAsync(Etudiant entity, CancellationToken ct = default)
    {
        _context.Etudiants.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Etudiants
                      .Where(e => e.Id == id)
                      .ExecuteDeleteAsync(ct);
    }
}
