using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = $"{ApplicationRole.Administrateur},{ApplicationRole.Enseignant},{ApplicationRole.Etudiant}")]
public sealed class EtudiantsController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private const int PageSize = 8;

    public EtudiantsController(IMediator mediator, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _mediator      = mediator;
        _userManager   = userManager;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        var etudiants = await _mediator.Send(new GetEtudiantsQuery(page, PageSize), ct);
        return View(new EtudiantsViewModel { Etudiants = etudiants, CurrentPage = page });
    }

    [HttpGet]
    public async Task<IActionResult> Table(int page, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        var etudiants = await _mediator.Send(new GetEtudiantsQuery(page, PageSize), ct);
        return PartialView("_EtudiantsTable", new EtudiantsViewModel { Etudiants = etudiants, CurrentPage = page });
    }

    [HttpGet]
    public IActionResult FormCreate()
        => PartialView("_FormCreate", new EtudiantsViewModel());

    [HttpGet]
    public async Task<IActionResult> FormEdit(int id, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(id), ct);
        EtudiantFormModel form = new();
        if (etudiant is not null)
        {
            form = new EtudiantFormModel
            {
                Id             = etudiant.Id,
                NumeroEtudiant = etudiant.NumeroEtudiant,
                Nom            = etudiant.Nom,
                Prenom         = etudiant.Prenom,
                Email          = etudiant.Email,
                Telephone      = etudiant.Telephone,
                Adresse        = etudiant.Adresse,
                DateNaissance  = etudiant.DateNaissance,
                DateInscription = etudiant.DateInscription
            };
        }
        return PartialView("_FormEdit", new EtudiantsViewModel
        {
            SelectedEtudiant = etudiant,
            Etudiant = form
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(int id, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(id), ct);
        return PartialView("_FormDelete", new EtudiantsViewModel
        {
            SelectedEtudiant = etudiant,
            EtudiantId = etudiant?.Id ?? 0
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDetails(int id, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(id), ct);
        BulletinEtudiant? bulletin = null;
        if (etudiant is not null)
            bulletin = await _mediator.Send(new GetEtudiantBulletinQuery(id), ct);
        return PartialView("_FormDetails", new EtudiantsViewModel
        {
            SelectedEtudiant = etudiant,
            Bulletin = bulletin
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] EtudiantFormModel form, CancellationToken ct)
    {
        // 1. Créer le compte Identity (EstActif=false, en attente validation admin)
        var user = new ApplicationUser
        {
            UserName       = form.Email,
            Email          = form.Email,
            Prenom         = form.Prenom,
            Nom            = form.Nom,
            PhoneNumber    = form.Telephone,
            EmailConfirmed = true,
            EstActif       = false
        };
        var defaultPassword = _configuration["Identity:DefaultPassword"] ?? "Changeme@1234!";
        var identityResult = await _userManager.CreateAsync(user, defaultPassword);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(" | ", identityResult.Errors.Select(e => e.Description));
            return Json(new { success = false, message = errors });
        }
        await _userManager.AddToRoleAsync(user, ApplicationRole.Etudiant);

        // 2. Créer la fiche étudiant liée
        var etudiant = new Etudiant
        {
            UserId        = user.Id,
            DateNaissance = form.DateNaissance,
            Adresse       = form.Adresse
        };
        var result = await _mediator.Send(new CreateEtudiantCommand(etudiant), ct);
        if (!result.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            return Json(new { success = false, message = result.ErrorMessage });
        }

        // 3. Mettre à jour le lien EtudiantId dans AspNetUsers
        user.EtudiantId = result.Value!.Id;
        await _userManager.UpdateAsync(user);

        return Json(new { success = true, message = $"Étudiant {user.Prenom} {user.Nom} créé avec succès." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] EtudiantFormModel form, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(form.Id), ct);
        if (etudiant is null)
            return Json(new { success = false, message = "Étudiant introuvable." });

        // Mettre à jour le compte Identity
        var user = await _userManager.FindByIdAsync(etudiant.UserId);
        if (user is not null)
        {
            user.Prenom      = form.Prenom;
            user.Nom         = form.Nom;
            user.PhoneNumber = form.Telephone;
            if (user.Email != form.Email)
            {
                user.Email          = form.Email;
                user.UserName       = form.Email;
                user.NormalizedEmail     = form.Email.ToUpperInvariant();
                user.NormalizedUserName  = form.Email.ToUpperInvariant();
            }
            await _userManager.UpdateAsync(user);
        }

        // Mettre à jour la fiche étudiant
        etudiant.DateNaissance = form.DateNaissance;
        etudiant.Adresse       = form.Adresse;

        var result = await _mediator.Send(new UpdateEtudiantCommand(etudiant), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Étudiant mis à jour avec succès." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int EtudiantId, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(EtudiantId), ct);
        var result = await _mediator.Send(new DeleteEtudiantCommand(EtudiantId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });

        if (etudiant is not null)
        {
            var user = await _userManager.FindByIdAsync(etudiant.UserId);
            if (user is not null) await _userManager.DeleteAsync(user);
        }
        return Json(new { success = true, message = "Étudiant supprimé avec succès." });
    }
}
