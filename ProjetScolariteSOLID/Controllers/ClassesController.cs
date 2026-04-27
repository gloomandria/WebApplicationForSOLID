using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Classes.Commands;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

public sealed class ClassesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<Filiere> _filiereRepo;
    private readonly IReferentielRepository<AnneeAcademique> _anneeRepo;
    private readonly IReferentielRepository<Niveau> _niveauRepo;

    public ClassesController(
        IMediator mediator,
        IReferentielRepository<Filiere> filiereRepo,
        IReferentielRepository<AnneeAcademique> anneeRepo,
        IReferentielRepository<Niveau> niveauRepo)
    {
        _mediator    = mediator;
        _filiereRepo = filiereRepo;
        _anneeRepo   = anneeRepo;
        _niveauRepo  = niveauRepo;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var classes = await _mediator.Send(new GetAllClassesQuery(), ct);
        return View(new ClassesViewModel { Classes = classes });
    }

    [HttpGet]
    public async Task<IActionResult> Table(CancellationToken ct)
    {
        var classes = await _mediator.Send(new GetAllClassesQuery(), ct);
        return PartialView("_ClassesTable", new ClassesViewModel { Classes = classes });
    }

    [HttpGet]
    public async Task<IActionResult> FormCreate(CancellationToken ct)
    {
        var (f, a, n) = await LoadListsAsync(ct);
        return PartialView("_FormCreate", new ClassesViewModel { FilieresList = f, AnneesAcadList = a, NiveauxList = n });
    }

    [HttpGet]
    public async Task<IActionResult> FormEdit(int id, CancellationToken ct)
    {
        var classe = await _mediator.Send(new GetClasseByIdQuery(id), ct);
        var (f, a, n) = await LoadListsAsync(ct);
        return PartialView("_FormEdit", new ClassesViewModel
        {
            SelectedClasse = classe,
            Classe = classe ?? new Classe(),
            FilieresList = f, AnneesAcadList = a, NiveauxList = n
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(int id, CancellationToken ct)
    {
        var classe = await _mediator.Send(new GetClasseByIdQuery(id), ct);
        return PartialView("_FormDelete", new ClassesViewModel
        {
            SelectedClasse = classe,
            ClasseId = classe?.Id ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] Classe Classe, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateClasseCommand(Classe), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = $"Classe \"{result.Value!.Nom}\" créée avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] Classe Classe, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateClasseCommand(Classe), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Classe mise à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int ClasseId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteClasseCommand(ClasseId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Classe supprimée avec succès." });
    }

    private async Task<(SelectList f, SelectList a, SelectList n)> LoadListsAsync(CancellationToken ct)
    {
        var f = new SelectList(await _filiereRepo.GetAllAsync(ct), nameof(Filiere.Id), nameof(Filiere.Libelle));
        var a = new SelectList(await _anneeRepo.GetAllAsync(ct), nameof(AnneeAcademique.Id), nameof(AnneeAcademique.Libelle));
        var n = new SelectList(await _niveauRepo.GetAllAsync(ct), nameof(Niveau.Id), nameof(Niveau.Libelle));
        return (f, a, n);
    }
}
