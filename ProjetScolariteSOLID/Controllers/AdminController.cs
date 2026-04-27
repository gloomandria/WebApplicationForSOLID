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

    // ── Liste des utilisateurs (shell DataTables) ─────────────────────────────
    public IActionResult Users() => View();

    // ── Endpoint DataJson Utilisateurs ────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> UsersDataJson(
        int    draw        = 1,
        int    start       = 0,
        int    length      = 10,
        string searchValue = "",
        int    sortCol     = 0,
        string sortDir     = "asc")
    {
        length = length is 10 or 20 or 50 ? length : 10;

        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchValue))
            users = users.Where(u => (u.Nom.Contains(searchValue) || u.Prenom.Contains(searchValue))
                                  || (u.Email != null && u.Email.Contains(searchValue)));

        var total = await users.CountAsync();

        users = (sortCol, sortDir.ToLower()) switch
        {
            (0, "asc")  => users.OrderBy(u => u.Nom).ThenBy(u => u.Prenom),
            (0, _)      => users.OrderByDescending(u => u.Nom).ThenByDescending(u => u.Prenom),
            (1, "asc")  => users.OrderBy(u => u.Email),
            (1, _)      => users.OrderByDescending(u => u.Email),
            (4, "asc")  => users.OrderBy(u => u.DateCreation),
            (4, _)      => users.OrderByDescending(u => u.DateCreation),
            _           => users.OrderBy(u => u.Nom).ThenBy(u => u.Prenom)
        };

        var paged = await users.Skip(start).Take(length).ToListAsync();

        var data = new List<object>();
        foreach (var u in paged)
        {
            var roles = await _userManager.GetRolesAsync(u);
            data.Add(new
            {
                id            = u.Id,
                nomComplet    = u.NomComplet,
                email         = u.Email ?? string.Empty,
                role          = roles.FirstOrDefault() ?? "-",
                emailConfirme = u.EmailConfirmed,
                estActif      = u.EstActif,
                dateCreation  = u.DateCreation.ToString("dd/MM/yyyy")
            });
        }

        return Json(new { draw, recordsTotal = total, recordsFiltered = total, data });
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

        return Json(new { success = true, message = $"Compte {(user.EstActif ? "activé" : "désactivé")}." });
    }

    // ── Envoyer un email de validation (activation + assignation mdp) ─────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendValidation(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        if (user.Email is null) return Json(new { success = false, message = "L'utilisateur n'a pas d'email." });

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

        return Json(new { success = true, message = $"Email de validation envoyé à {user.Email}." });
    }

    // ── Assigner un rôle (page complète — conservé pour compatibilité) ────────
    [HttpGet]
    public async Task<IActionResult> AssignRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        return View(new AssignRoleViewModel
        {
            UserId     = user.Id,
            NomComplet = user.NomComplet,
            Email      = user.Email ?? string.Empty,
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

    // ── Formulaire d'édition de rôle (partial pour modale AJAX) ──────────────
    [HttpGet]
    public async Task<IActionResult> FormEditRole(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        return PartialView("_FormEditRole", new AssignRoleViewModel
        {
            UserId     = user.Id,
            NomComplet = user.NomComplet,
            Email      = user.Email ?? string.Empty,
            RoleActuel = roles.FirstOrDefault() ?? string.Empty
        });
    }

    // ── Sauvegarde AJAX du rôle ───────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoleAjax([FromForm] string userId, [FromForm] string nouveauRole)
    {
        if (string.IsNullOrWhiteSpace(nouveauRole))
            return Json(new { success = false, message = "Veuillez choisir un rôle." });

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Json(new { success = false, message = "Utilisateur introuvable." });

        var rolesActuels = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, rolesActuels);
        await _userManager.AddToRoleAsync(user, nouveauRole);

        return Json(new { success = true, message = $"Rôle '{nouveauRole}' assigné à {user.NomComplet}." });
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

    // ── File d'emails (shell DataTables) ──────────────────────────────────────
    public IActionResult EmailQueue() => View();

    // ── Endpoint DataJson EmailQueue ──────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> EmailQueueDataJson(
        int    draw        = 1,
        int    start       = 0,
        int    length      = 20,
        string searchValue = "",
        int    sortCol     = 0,
        string sortDir     = "desc",
        CancellationToken ct = default)
    {
        length = length is 10 or 20 or 50 or 100 ? length : 20;

        var (items, filteredTotal) = await _emailQueue.GetAllPagedAsync(start, length, searchValue, sortCol, sortDir, ct);
        var grandTotal             = await _emailQueue.CountAsync(ct);

        var data = items.Select(e => new
        {
            id            = e.Id,
            destinataire  = e.Destinataire,
            sujet         = e.Sujet,
            statut        = e.Statut.ToString(),
            nbTentatives  = e.NbTentatives,
            dateCreation  = e.DateCreation.ToString("dd/MM/yyyy HH:mm"),
            dateEnvoi     = e.DateEnvoi?.ToString("dd/MM/yyyy HH:mm") ?? "-",
            messageErreur = e.MessageErreur ?? string.Empty
        });

        return Json(new { draw, recordsTotal = grandTotal, recordsFiltered = filteredTotal, data });
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
