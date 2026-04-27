namespace ProjetScolariteSOLID.Domain.Repositories;

/// <summary>
/// Interface dédiée aux référentiels simples (Id + Libelle) — lecture et écriture.
/// </summary>
public interface IReferentielRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T> CreateAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
