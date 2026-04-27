using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Commands;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = $"{ApplicationRole.Administrateur},{ApplicationRole.Enseignant}")]
public sealed class MatieresController : Controller
{
    private readonly IMediator _mediator;

    public MatieresController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        pageSize = pageSize is 10 or 20 or 30 or 50 ? pageSize : 10;
        var matieres = await _mediator.Send(new GetMatieresPagedQuery(page, pageSize), ct);
        return View(new MatieresViewModel { Matieres = matieres, CurrentPage = page, PageSize = pageSize });
    }

    [HttpGet]
    public async Task<IActionResult> Table(int page, int pageSize = 10, CancellationToken ct = default)
    {
        page     = page < 1 ? 1 : page;
        pageSize = pageSize is 10 or 20 or 30 or 50 ? pageSize : 10;
        var matieres = await _mediator.Send(new GetMatieresPagedQuery(page, pageSize), ct);
        return PartialView("_MatieresTable", new MatieresViewModel { Matieres = matieres, CurrentPage = page, PageSize = pageSize });
    }

    [HttpGet]
    public async Task<IActionResult> DataJson(int draw, int start, int length, string searchValue = "", int sortCol = 0, string sortDir = "asc", CancellationToken ct = default)
    {
        length = length is 10 or 20 or 30 or 50 ? length : 10;
        int page = (start / length) + 1;
        var result = await _mediator.Send(new GetMatieresPagedQuery(page, length, searchValue, sortCol, sortDir), ct);
        var data = result.Items.Select(m => new
        {
            id            = m.Id,
            code          = m.Code,
            intitule      = m.Intitule,
            coefficient   = m.Coefficient,
            volumeHoraire = m.VolumeHoraire,
            enseignant    = m.Enseignant?.NomComplet ?? "—"
        });
        return Json(new { draw, recordsTotal = result.TotalCount, recordsFiltered = result.TotalCount, data });
    }

    [HttpGet]
    public async Task<IActionResult> FormCreate(CancellationToken ct)
    {
        var list = await LoadEnseignantsAsync(ct);
        return PartialView("_FormCreate", new MatieresViewModel { EnseignantsList = list });
    }

    [HttpGet]
    public async Task<IActionResult> FormEdit(int id, CancellationToken ct)
    {
        var matiere = await _mediator.Send(new GetMatiereByIdQuery(id), ct);
        var list = await LoadEnseignantsAsync(ct);
        return PartialView("_FormEdit", new MatieresViewModel
        {
            SelectedMatiere = matiere,
            Matiere = matiere ?? new Matiere(),
            EnseignantsList = list
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(int id, CancellationToken ct)
    {
        var matiere = await _mediator.Send(new GetMatiereByIdQuery(id), ct);
        return PartialView("_FormDelete", new MatieresViewModel
        {
            SelectedMatiere = matiere,
            MatiereId = matiere?.Id ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] Matiere Matiere, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateMatiereCommand(Matiere), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = $"Matière \"{result.Value!.Intitule}\" créée avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] Matiere Matiere, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateMatiereCommand(Matiere), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Matière mise à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int MatiereId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteMatiereCommand(MatiereId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Matière supprimée avec succès." });
    }

    private async Task<SelectList> LoadEnseignantsAsync(CancellationToken ct)
    {
        var enseignants = await _mediator.Send(new GetAllEnseignantsQuery(), ct);
        return new SelectList(enseignants, nameof(Enseignant.Id), nameof(Enseignant.NomComplet));
    }
}
