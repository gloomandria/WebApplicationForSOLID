using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Infrastructure.Data;

namespace ProjetScolariteSOLID.Infrastructure.Auth;

public sealed class PermissionService : IPermissionService
{
    private readonly ScolariteDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionService(ScolariteDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    public async Task<RolePermission?> GetAsync(string roleId, string ressource, CancellationToken ct = default)
        => await _db.RolePermissions
                    .FirstOrDefaultAsync(p => p.RoleId == roleId && p.Ressource == ressource, ct);

    public async Task<IReadOnlyList<RolePermission>> GetByRoleAsync(string roleId, CancellationToken ct = default)
        => await _db.RolePermissions.Where(p => p.RoleId == roleId).ToListAsync(ct);

    public async Task<IReadOnlyList<RolePermission>> GetAllAsync(CancellationToken ct = default)
        => await _db.RolePermissions.Include(p => p.Role).ToListAsync(ct);

    public async Task UpsertAsync(string roleId, string ressource, bool peutVoir, bool peutEditer, bool peutSupprimer, CancellationToken ct = default)
    {
        var perm = await _db.RolePermissions
                            .FirstOrDefaultAsync(p => p.RoleId == roleId && p.Ressource == ressource, ct);
        if (perm is null)
        {
            _db.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId, Ressource = ressource,
                PeutVoir = peutVoir, PeutEditer = peutEditer, PeutSupprimer = peutSupprimer
            });
        }
        else
        {
            perm.PeutVoir      = peutVoir;
            perm.PeutEditer    = peutEditer;
            perm.PeutSupprimer = peutSupprimer;
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> HasPermissionAsync(string userId, string ressource, string action, CancellationToken ct = default)
    {
        var user  = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(ApplicationRole.Administrateur)) return true;

        var roleIds = await _db.Roles
                               .Where(r => roles.Contains(r.Name!))
                               .Select(r => r.Id)
                               .ToListAsync(ct);

        var perms = await _db.RolePermissions
                             .Where(p => roleIds.Contains(p.RoleId) && p.Ressource == ressource)
                             .ToListAsync(ct);

        return action switch
        {
            "voir"      => perms.Any(p => p.PeutVoir),
            "editer"    => perms.Any(p => p.PeutEditer),
            "supprimer" => perms.Any(p => p.PeutSupprimer),
            _           => false
        };
    }
}
