namespace ProjetScolariteSOLID.Domain.Repositories;

/// <summary>
/// ISP — Interface dédiée uniquement à l'écriture (générique).
/// </summary>
public interface IWriteRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
