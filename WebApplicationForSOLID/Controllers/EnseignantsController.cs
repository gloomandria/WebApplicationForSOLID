using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

public sealed class EnseignantsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<Specialite> _specialiteRepo;
    private readonly IReferentielRepository<Grade> _gradeRepo;
    private const int PageSize = 8;

    public EnseignantsController(
        IMediator mediator,
        IReferentielRepository<Specialite> specialiteRepo,
        IReferentielRepository<Grade> gradeRepo)
    {
        _mediator       = mediator;
        _specialiteRepo = specialiteRepo;
        _gradeRepo      = gradeRepo;
    }

    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        var enseignants = await _mediator.Send(new GetEnseignantsQuery(page, PageSize), ct);
        return View(new EnseignantsViewModel { Enseignants = enseignants, CurrentPage = page });
    }

    [HttpGet]
    public async Task<IActionResult> Table(int page, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        var enseignants = await _mediator.Send(new GetEnseignantsQuery(page, PageSize), ct);
        return PartialView("_EnseignantsTable", new EnseignantsViewModel { Enseignants = enseignants, CurrentPage = page });
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
        return PartialView("_FormEdit", new EnseignantsViewModel
        {
            SelectedEnseignant = enseignant,
            Enseignant = enseignant ?? new Enseignant(),
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] Enseignant Enseignant, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateEnseignantCommand(Enseignant), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = $"Enseignant {result.Value!.NomComplet} créé avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] Enseignant Enseignant, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateEnseignantCommand(Enseignant), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Enseignant mis à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int EnseignantId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteEnseignantCommand(EnseignantId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Enseignant supprimé avec succès." });
    }

    private async Task<(SelectList spec, SelectList grad)> LoadListsAsync(CancellationToken ct)
    {
        var spec = new SelectList(await _specialiteRepo.GetAllAsync(ct), nameof(Specialite.Id), nameof(Specialite.Libelle));
        var grad = new SelectList(await _gradeRepo.GetAllAsync(ct), nameof(Grade.Id), nameof(Grade.Libelle));
        return (spec, grad);
    }
}
