using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Application.CQRS.Notes.Commands;
using ProjetScolariteSOLID.Application.CQRS.Notes.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

public sealed class NotesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<TypeEvaluationRef> _typeEvalRepo;
    private const int PageSize = 10;

    public NotesController(IMediator mediator, IReferentielRepository<TypeEvaluationRef> typeEvalRepo)
    {
        _mediator     = mediator;
        _typeEvalRepo = typeEvalRepo;
    }

    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        var notes = await _mediator.Send(new GetNotesQuery(page, PageSize), ct);
        return View(new NotesViewModel { Notes = notes, CurrentPage = page });
    }

    [HttpGet]
    public async Task<IActionResult> Table(int page, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        var notes = await _mediator.Send(new GetNotesQuery(page, PageSize), ct);
        return PartialView("_NotesTable", new NotesViewModel { Notes = notes, CurrentPage = page });
    }

    [HttpGet]
    public async Task<IActionResult> FormCreate(CancellationToken ct)
    {
        var (e, m, t) = await LoadListsAsync(ct);
        return PartialView("_FormCreate", new NotesViewModel { EtudiantsList = e, MatieresList = m, TypesEvalList = t });
    }

    [HttpGet]
    public async Task<IActionResult> FormEdit(int id, CancellationToken ct)
    {
        var note = await _mediator.Send(new GetNoteByIdQuery(id), ct);
        var (e, m, t) = await LoadListsAsync(ct);
        return PartialView("_FormEdit", new NotesViewModel
        {
            SelectedNote = note,
            Note = note ?? new Note(),
            EtudiantsList = e, MatieresList = m, TypesEvalList = t
        });
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(int id, CancellationToken ct)
    {
        var note = await _mediator.Send(new GetNoteByIdQuery(id), ct);
        return PartialView("_FormDelete", new NotesViewModel
        {
            SelectedNote = note,
            NoteId = note?.Id ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] Note Note, CancellationToken ct)
    {
        var result = await _mediator.Send(new AjouterNoteCommand(Note), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Note ajoutée avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] Note Note, CancellationToken ct)
    {
        var result = await _mediator.Send(new ModifierNoteCommand(Note), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Note mise à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] int NoteId, CancellationToken ct)
    {
        var result = await _mediator.Send(new SupprimerNoteCommand(NoteId), ct);
        if (!result.IsSuccess) return Json(new { success = false, message = result.ErrorMessage });
        return Json(new { success = true, message = "Note supprimée avec succès." });
    }

    private async Task<(SelectList e, SelectList m, SelectList t)> LoadListsAsync(CancellationToken ct)
    {
        var etudiants = await _mediator.Send(new GetAllEtudiantsQuery(), ct);
        var matieres  = await _mediator.Send(new GetAllMatieresQuery(), ct);
        var typesEval = await _typeEvalRepo.GetAllAsync(ct);
        var e = new SelectList(etudiants,  nameof(Etudiant.Id),          nameof(Etudiant.NomComplet));
        var m = new SelectList(matieres,   nameof(Matiere.Id),           nameof(Matiere.Intitule));
        var t = new SelectList(typesEval,  nameof(TypeEvaluationRef.Id), nameof(TypeEvaluationRef.Libelle));
        return (e, m, t);
    }
}
