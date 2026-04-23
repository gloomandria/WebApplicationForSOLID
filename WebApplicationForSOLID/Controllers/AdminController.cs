using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;
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
    private readonly IEmailTemplateService         _emailTemplateService;

    // Ressources gérées par la matrice de permissions
    private static readonly string[] Ressources =
        ["Etudiants", "Enseignants", "Matieres", "Classes", "Inscriptions", "Notes", "Referentiels"];

    public AdminController(
        UserManager<ApplicationUser>  userManager,
        RoleManager<ApplicationRole>  roleManager,
        IPermissionService            permissionService,
        IEmailQueueService            emailQueue,
        IEmailTemplateService         emailTemplateService)
    {
        _userManager          = userManager;
        _roleManager          = roleManager;
        _permissionService    = permissionService;
        _emailQueue           = emailQueue;
        _emailTemplateService = emailTemplateService;
    }

    // ── Dashboard back-office ─────────────────────────────────────────────────
    public IActionResult Index() => View();

    // ── Liste des utilisateurs ────────────────────────────────────────────────
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
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

        user.EstActif = !user.EstActif;
        await _userManager.UpdateAsync(user);

        if (user.Email is not null)
        {
            var templateCode = user.EstActif ? EmailTemplateCode.CompteActive : EmailTemplateCode.CompteDesactive;
            var variables = new Dictionary<string, string>
            {
                ["NomComplet"] = user.NomComplet,
                ["Email"]      = user.Email
            };
            var fallbackSujet = user.EstActif
                ? "Votre compte a été activé — Gestion Scolarité"
                : "Votre compte a été désactivé — Gestion Scolarité";
            var fallbackCorps = user.EstActif
                ? $"""<h2>Bonjour {user.NomComplet},</h2><p>Votre compte a été <strong>activé</strong>.</p>"""
                : $"""<h2>Bonjour {user.NomComplet},</h2><p>Votre compte a été <strong>désactivé</strong>.</p>""";
            await EnqueueFromTemplateAsync(templateCode, user.Email, variables, fallbackSujet, fallbackCorps);
        }

        TempData["Success"] = $"Compte {(user.EstActif ? "activé" : "désactivé")}.";
        return RedirectToAction("Users");
    }

    // ── Envoyer un email de validation (activation + assignation mdp) ─────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendValidation(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        if (user.Email is null) { TempData["Error"] = "L'utilisateur n'a pas d'email."; return RedirectToAction("Users"); }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var activateUrl = Url.Action("ActivateAccount", "Account",
                            new { userId = user.Id, token }, Request.Scheme)!;

        var variables = new Dictionary<string, string>
        {
            ["NomComplet"] = user.NomComplet,
            ["Email"]      = user.Email,
            ["Lien"]       = activateUrl
        };

        await EnqueueFromTemplateAsync(
            EmailTemplateCode.ValidationCompte,
            user.Email,
            variables,
            "Activation de votre compte — Gestion Scolarité",
            $"""<h2>Bonjour {user.NomComplet},</h2><p>Activez votre compte : <a href="{activateUrl}">cliquez ici</a></p>""");

        TempData["Success"] = $"Email de validation envoyé à {user.Email}.";
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

    private async Task EnqueueFromTemplateAsync(
        string templateCode,
        string destinataire,
        Dictionary<string, string> variables,
        string fallbackSujet,
        string fallbackCorps,
        CancellationToken ct = default)
    {
        var template = await _emailTemplateService.GetByCodeAsync(templateCode, ct);
        if (template is not null)
        {
            var (sujet, corps) = template.Appliquer(variables);
            await _emailQueue.EnqueueAsync(destinataire, sujet, corps, ct: ct);
        }
        else
        {
            await _emailQueue.EnqueueAsync(destinataire, fallbackSujet, fallbackCorps, ct: ct);
        }
    }
}
