namespace ProjetScolariteSOLID.Domain.Repositories;

/// <summary>
/// ISP — Interface dédiée à la lecture des référentiels simples (Id + Libelle).
/// </summary>
public interface IReferentielRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
}
