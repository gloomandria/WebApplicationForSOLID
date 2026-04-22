using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Commands;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = $"{ApplicationRole.Administrateur},{ApplicationRole.Enseignant}")]
public sealed class InscriptionsController : Controller
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<StatutInscriptionRef> _statutRepo;
    private const int PageSize = 10;

    public InscriptionsController(IMediator mediator, IReferentielRepository<StatutInscriptionRef> statutRepo)
    {
        _mediator   = mediator;
        _statutRepo = statutRepo;
    }

    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        var inscriptions = await _mediator.Send(new GetInscriptionsQuery(page, PageSize), ct);
        return View(new InscriptionsViewModel { Inscriptions = inscriptions, CurrentPage = page });
    }

    [HttpGet]
    public async Task<IActionResult> Table(int page, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        var inscriptions = await _mediator.Send(new GetInscriptionsQuery(page, PageSize), ct);
        return PartialView("_InscriptionsTable", new InscriptionsViewModel { Inscriptions = inscriptions, CurrentPage = page });
    }

    [HttpGet]
    public async Task<IActionResult> FormCreate(CancellationToken ct)
    {
        var (e, c) = await LoadCreateListsAsync(ct);
        return PartialView("_FormCreate", new InscriptionsViewModel { EtudiantsList = e, ClassesList = c });
    }

    [HttpGet]
    public async Task<IActionResult> FormEditStatut(int id, CancellationToken ct)
    {
        var inscription = await _mediator.Send(new GetInscriptionByIdQuery(id), ct);
        var statuts = await LoadStatutsAsync(ct);
        return PartialView("_FormEditStatut", new InscriptionsViewModel
        {
            SelectedInscription = inscription,
            InscriptionId = inscription?.Id ?? 0,
            StatutId = inscription?.StatutId ?? 0,
            StatutsList = statuts
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(int id, CancellationToken ct)
    {
        var inscription = await _mediator.Send(new GetInscriptionByIdQuery(id), ct);
        return PartialView("_FormDelete", new InscriptionsViewModel
        {
            SelectedInscription = inscription,
            InscriptionId = inscription?.Id ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] int EtudiantId, [FromForm] int ClasseId, CancellationToken ct)
    {
        var result = await _mediator.Send(new InscrireEtudiantCommand(EtudiantId, ClasseId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Inscription créée avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStatutAjax([FromForm] int InscriptionId, [FromForm] int StatutId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ModifierStatutInscriptionCommand(InscriptionId, StatutId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Statut mis à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int InscriptionId, CancellationToken ct)
    {
        var result = await _mediator.Send(new SupprimerInscriptionCommand(InscriptionId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Inscription supprimée avec succès." });
    }

    private async Task<(SelectList e, SelectList c)> LoadCreateListsAsync(CancellationToken ct)
    {
        var etudiants = await _mediator.Send(new GetAllEtudiantsQuery(), ct);
        var classes   = await _mediator.Send(new GetAllClassesQuery(), ct);
        var e = new SelectList(etudiants, nameof(Etudiant.Id), nameof(Etudiant.NomComplet));
        var c = new SelectList(classes,   nameof(Classe.Id),   nameof(Classe.Nom));
        return (e, c);
    }

    private async Task<SelectList> LoadStatutsAsync(CancellationToken ct)
        => new SelectList(await _statutRepo.GetAllAsync(ct), nameof(StatutInscriptionRef.Id), nameof(StatutInscriptionRef.Libelle));
}
