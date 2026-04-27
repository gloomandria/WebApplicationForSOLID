using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = $"{ApplicationRole.Administrateur},{ApplicationRole.Enseignant}")]
public sealed class EnseignantsController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IReferentielRepository<Specialite> _specialiteRepo;
    private readonly IReferentielRepository<Grade> _gradeRepo;
    private readonly IConfiguration _configuration;
    private const int DefaultPageSize = 10;

    public EnseignantsController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IReferentielRepository<Specialite> specialiteRepo,
        IReferentielRepository<Grade> gradeRepo,
        IConfiguration configuration)
    {
        _mediator       = mediator;
        _userManager    = userManager;
        _specialiteRepo = specialiteRepo;
        _gradeRepo      = gradeRepo;
        _configuration  = configuration;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = DefaultPageSize, CancellationToken ct = default)
    {
        pageSize = pageSize is 10 or 20 or 30 or 50 ? pageSize : DefaultPageSize;
        var enseignants = await _mediator.Send(new GetEnseignantsQuery(page, pageSize), ct);
        return View(new EnseignantsViewModel { Enseignants = enseignants, CurrentPage = page, PageSize = pageSize });
    }

    [HttpGet]
    public async Task<IActionResult> Table(int page, int pageSize = DefaultPageSize, CancellationToken ct = default)
    {
        page     = page < 1 ? 1 : page;
        pageSize = pageSize is 10 or 20 or 30 or 50 ? pageSize : DefaultPageSize;
        var enseignants = await _mediator.Send(new GetEnseignantsQuery(page, pageSize), ct);
        return PartialView("_EnseignantsTable", new EnseignantsViewModel { Enseignants = enseignants, CurrentPage = page, PageSize = pageSize });
    }

    [HttpGet]
    public async Task<IActionResult> DataJson(int draw, int start, int length, string searchValue = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        length = length is 10 or 20 or 30 or 50 ? length : DefaultPageSize;
        int page = (start / length) + 1;
        var result = await _mediator.Send(new GetEnseignantsQuery(page, length, searchValue, sortCol, sortDir), ct);
        var data = result.Items.Select(e => new
        {
            id         = e.Id,
            matricule  = e.Matricule,
            nomComplet = e.NomComplet,
            email      = e.Email,
            specialite = e.Specialite?.Libelle ?? "",
            grade      = e.Grade?.Libelle ?? ""
        });
        return Json(new { draw, recordsTotal = result.TotalCount, recordsFiltered = result.TotalCount, data });
    }

    [HttpGet]
    public async Task<IActionResult> FormCreate(CancellationToken ct)
    {
        var (spec, grad) = await LoadListsAsync(ct);
        return PartialView("_FormCreate", new EnseignantsViewModel { SpecialitesList = spec, GradesList = grad });
    }

    [HttpGet]
    public async Task<IActionResult> FormEdit(int id, CancellationToken ct)
    {
        var enseignant = await _mediator.Send(new GetEnseignantByIdQuery(id), ct);
        var (spec, grad) = await LoadListsAsync(ct);
        EnseignantFormModel form = new();
        if (enseignant is not null)
        {
            form = new EnseignantFormModel
            {
                Id          = enseignant.Id,
                Matricule   = enseignant.Matricule,
                Nom         = enseignant.Nom,
                Prenom      = enseignant.Prenom,
                Email       = enseignant.Email,
                Telephone   = enseignant.Telephone,
                SpecialiteId = enseignant.SpecialiteId,
                GradeId     = enseignant.GradeId,
                DateEmbauche = enseignant.DateEmbauche
            };
        }
        return PartialView("_FormEdit", new EnseignantsViewModel
        {
            SelectedEnseignant = enseignant,
            Enseignant = form,
            SpecialitesList = spec,
            GradesList = grad
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(int id, CancellationToken ct)
    {
        var enseignant = await _mediator.Send(new GetEnseignantByIdQuery(id), ct);
        return PartialView("_FormDelete", new EnseignantsViewModel
        {
            SelectedEnseignant = enseignant,
            EnseignantId = enseignant?.Id ?? 0
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] EnseignantFormModel form, CancellationToken ct)
    {
        // 1. Créer le compte Identity
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
        await _userManager.AddToRoleAsync(user, ApplicationRole.Enseignant);

        // 2. Créer la fiche enseignant liée
        var enseignant = new Enseignant
        {
            UserId      = user.Id,
            SpecialiteId = form.SpecialiteId,
            GradeId     = form.GradeId
        };
        var result = await _mediator.Send(new CreateEnseignantCommand(enseignant), ct);
        if (!result.IsSuccess)
        {
            await _userManager.DeleteAsync(user);
            return Json(new { success = false, message = result.ErrorMessage });
        }

        // 3. Mettre à jour le lien EnseignantId dans AspNetUsers
        user.EnseignantId = result.Value!.Id;
        await _userManager.UpdateAsync(user);

        return Json(new { success = true, message = $"Enseignant {user.Prenom} {user.Nom} créé avec succès." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] EnseignantFormModel form, CancellationToken ct)
    {
        var enseignant = await _mediator.Send(new GetEnseignantByIdQuery(form.Id), ct);
        if (enseignant is null)
            return Json(new { success = false, message = "Enseignant introuvable." });

        // Mettre à jour le compte Identity
        var user = await _userManager.FindByIdAsync(enseignant.UserId);
        if (user is not null)
        {
            user.Prenom      = form.Prenom;
            user.Nom         = form.Nom;
            user.PhoneNumber = form.Telephone;
            await _userManager.UpdateAsync(user);
            if (!string.Equals(user.Email, form.Email, StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.SetEmailAsync(user, form.Email);
                await _userManager.SetUserNameAsync(user, form.Email);
            }
        }

        // Mettre à jour la fiche enseignant
        enseignant.SpecialiteId = form.SpecialiteId;
        enseignant.GradeId      = form.GradeId;

        var result = await _mediator.Send(new UpdateEnseignantCommand(enseignant), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Enseignant mis à jour avec succès." });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int EnseignantId, CancellationToken ct)
    {
        var enseignant = await _mediator.Send(new GetEnseignantByIdQuery(EnseignantId), ct);
        var result = await _mediator.Send(new DeleteEnseignantCommand(EnseignantId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });

        if (enseignant is not null)
        {
            var user = await _userManager.FindByIdAsync(enseignant.UserId);
            if (user is not null) await _userManager.DeleteAsync(user);
        }
        return Json(new { success = true, message = "Enseignant supprimé avec succès." });
    }

    private async Task<(SelectList spec, SelectList grad)> LoadListsAsync(CancellationToken ct)
    {
        var spec = new SelectList(await _specialiteRepo.GetAllAsync(ct), nameof(Specialite.Id), nameof(Specialite.Libelle));
        var grad = new SelectList(await _gradeRepo.GetAllAsync(ct), nameof(Grade.Id), nameof(Grade.Libelle));
        return (spec, grad);
    }
}
