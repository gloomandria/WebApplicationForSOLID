using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.ViewModels.Admin;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = ApplicationRole.Administrateur)]
public sealed class AdminController : Controller
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly RoleManager<ApplicationRole>  _roleManager;
    private readonly IPermissionService            _permissionService;
    private readonly IEmailQueueService            _emailQueue;

    // Ressources gérées par la matrice de permissions
    private static readonly string[] Ressources =
        ["Etudiants", "Enseignants", "Matieres", "Classes", "Inscriptions", "Notes", "Referentiels"];

    public AdminController(
        UserManager<ApplicationUser>  userManager,
        RoleManager<ApplicationRole>  roleManager,
        IPermissionService            permissionService,
        IEmailQueueService            emailQueue)
    {
        _userManager       = userManager;
        _roleManager       = roleManager;
        _permissionService = permissionService;
        _emailQueue        = emailQueue;
    }

    // ── Dashboard back-office ─────────────────────────────────────────────────
    public IActionResult Index() => View();

    // ── Liste des utilisateurs ────────────────────────────────────────────────
    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        var rows  = new List<UserRowViewModel>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            rows.Add(new UserRowViewModel
            {
                Id           = u.Id,
                NomComplet   = u.NomComplet,
                Email        = u.Email ?? string.Empty,
                Role         = roles.FirstOrDefault() ?? "-",
                EstActif     = u.EstActif,
                EmailConfirme = u.EmailConfirmed,
                DateCreation = u.DateCreation
            });
        }
        return View(new UserListViewModel { Users = rows });
    }

    // ── Activer / Désactiver un utilisateur ───────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var wasInactive = !user.EstActif;
        user.EstActif = !user.EstActif;
        await _userManager.UpdateAsync(user);

        // Envoyer un email à l'utilisateur lorsque son compte est activé
        if (wasInactive && user.EstActif && user.Email is not null)
        {
            await _emailQueue.EnqueueAsync(
                destinataire: user.Email,
                sujet:        "Votre compte a été validé — Gestion Scolarité",
                corps:        BuildAccountActivatedEmailHtml(user.NomComplet));
        }

        TempData["Success"] = $"Compte {(user.EstActif ? "activé" : "désactivé")}.";
        return RedirectToAction("Users");
    }

    // ── Assigner un rôle ──────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> AssignRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        return View(new AssignRoleViewModel
        {
            UserId    = user.Id,
            NomComplet = user.NomComplet,
            Email     = user.Email ?? string.Empty,
            RoleActuel = roles.FirstOrDefault() ?? string.Empty
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null) return NotFound();

        var rolesActuels = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, rolesActuels);
        await _userManager.AddToRoleAsync(user, model.NouveauRole);

        TempData["Success"] = $"Rôle '{model.NouveauRole}' assigné à {user.NomComplet}.";
        return RedirectToAction("Users");
    }

    // ── Matrice de permissions ────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Permissions(CancellationToken ct)
    {
        var allPerms = await _permissionService.GetAllAsync(ct);
        var roles    = _roleManager.Roles
                           .Where(r => r.Name != ApplicationRole.Administrateur)
                           .ToList();

        var rows = roles.Select(role => new RolePermissionsRow
        {
            RoleId   = role.Id,
            RoleName = role.Name ?? string.Empty,
            Permissions = Ressources.ToDictionary(
                r => r,
                r => allPerms.FirstOrDefault(p => p.RoleId == role.Id && p.Ressource == r)
                     ?? new RolePermission { RoleId = role.Id, Ressource = r })
        }).ToList();

        return View(new PermissionMatrixViewModel { Ressources = Ressources, Rows = rows });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePermissions(
        [FromForm] string roleId,
        [FromForm] string ressource,
        [FromForm] bool peutVoir,
        [FromForm] bool peutEditer,
        [FromForm] bool peutSupprimer,
        CancellationToken ct)
    {
        await _permissionService.UpsertAsync(roleId, ressource, peutVoir, peutEditer, peutSupprimer, ct);
        return Ok();
    }

    // ── File d'emails ─────────────────────────────────────────────────────────
    public async Task<IActionResult> EmailQueue(int page = 1, CancellationToken ct = default)
    {
        const int pageSize = 20;
        var emails = await _emailQueue.GetAllAsync(page, pageSize, ct);
        var total  = await _emailQueue.CountAsync(ct);
        return View(new EmailQueueListViewModel
        {
            Emails   = emails,
            Page     = page,
            PageSize = pageSize,
            Total    = total
        });
    }

    private static string BuildAccountActivatedEmailHtml(string nomComplet) => $"""
        <h2>Bonjour {nomComplet},</h2>
        <p>Votre compte sur le portail <strong>Gestion Scolarité</strong> a été <strong>activé</strong> par un administrateur.</p>
        <p>Vous pouvez désormais vous connecter avec vos identifiants.</p>
        """;
}
