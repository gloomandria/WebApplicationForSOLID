using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

public sealed class EtudiantsController : Controller
{
    private readonly IMediator _mediator;
    private const int PageSize = 8;

    public EtudiantsController(IMediator mediator) => _mediator = mediator;

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
        return PartialView("_FormEdit", new EtudiantsViewModel
        {
            SelectedEtudiant = etudiant,
            Etudiant = etudiant ?? new Etudiant()
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] Etudiant Etudiant, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateEtudiantCommand(Etudiant), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = $"Étudiant {result.Value!.NomComplet} créé avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] Etudiant Etudiant, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateEtudiantCommand(Etudiant), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Étudiant mis à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int EtudiantId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteEtudiantCommand(EtudiantId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Étudiant supprimé avec succès." });
    }
}
