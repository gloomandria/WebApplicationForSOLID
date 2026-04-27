using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Application.Contracts;

public interface IPermissionService
{
    Task<RolePermission?> GetAsync(string roleId, string ressource, CancellationToken ct = default);
    Task<IReadOnlyList<RolePermission>> GetByRoleAsync(string roleId, CancellationToken ct = default);
    Task<IReadOnlyList<RolePermission>> GetAllAsync(CancellationToken ct = default);
    Task UpsertAsync(string roleId, string ressource, bool peutVoir, bool peutEditer, bool peutSupprimer, CancellationToken ct = default);

    /// <summary>Vérifie si l'utilisateur courant a la permission demandée sur la ressource.</summary>
    Task<bool> HasPermissionAsync(string userId, string ressource, string action, CancellationToken ct = default);
}
