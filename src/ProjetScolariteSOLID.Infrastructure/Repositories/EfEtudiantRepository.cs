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
                         .AsNoTracking()
                         .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Etudiant>> GetAllAsync(CancellationToken ct = default)
        => await _context.Etudiants
                         .AsNoTracking()
                         .OrderBy(e => e.Nom).ThenBy(e => e.Prenom)
                         .ToListAsync(ct);

    public async Task<PagedResult<Etudiant>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Etudiants
                            .AsNoTracking()
                            .OrderBy(e => e.Nom).ThenBy(e => e.Prenom);

        var totalCount = await query.CountAsync(ct);
        var items = await query
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

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => _context.Etudiants.AnyAsync(
               e => e.Email == email && e.Id != excludeId, ct);

    public async Task<IReadOnlyList<Etudiant>> GetByClasseAsync(int classeId, CancellationToken ct = default)
        => await _context.Inscriptions
                         .AsNoTracking()
                         .Where(i => i.ClasseId == classeId
                                && i.Statut != null && i.Statut.Libelle == "Active")
                         .Include(i => i.Etudiant)
                         .Select(i => i.Etudiant!)
                         .OrderBy(e => e.Nom)
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
