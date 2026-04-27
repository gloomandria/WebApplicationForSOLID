using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Commands;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

public sealed class MatieresController : Controller
{
    private readonly IMediator _mediator;

    public MatieresController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var matieres = await _mediator.Send(new GetAllMatieresQuery(), ct);
        return View(new MatieresViewModel { Matieres = matieres });
    }

    [HttpGet]
    public async Task<IActionResult> Table(CancellationToken ct)
    {
        var matieres = await _mediator.Send(new GetAllMatieresQuery(), ct);
        return PartialView("_MatieresTable", new MatieresViewModel { Matieres = matieres });
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
