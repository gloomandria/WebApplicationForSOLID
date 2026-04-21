namespace WebApplicationForSOLID.Domain.Repositories;

/// <summary>
/// ISP — Interface dédiée uniquement à la lecture (générique).
/// </summary>
public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
}
