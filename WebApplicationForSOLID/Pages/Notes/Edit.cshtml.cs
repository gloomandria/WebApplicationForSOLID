using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Application.CQRS.Notes.Commands;
using ProjetScolariteSOLID.Application.CQRS.Notes.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;

namespace ProjetScolariteSOLID.Pages.Notes;

public sealed class EditModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<TypeEvaluationRef> _typeEvalRepo;

    public EditModel(IMediator mediator, IReferentielRepository<TypeEvaluationRef> typeEvalRepo)
    {
        _mediator     = mediator;
        _typeEvalRepo = typeEvalRepo;
    }

    [BindProperty]
    public Note Note { get; set; } = new();

    public SelectList EtudiantsList { get; private set; } = default!;
    public SelectList MatieresList  { get; private set; } = default!;
    public SelectList TypesEvalList { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var note = await _mediator.Send(new GetNoteByIdQuery(id), ct);
        if (note is null) return NotFound();
        Note = note;
        await LoadListsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadListsAsync(ct); return Page(); }

        var result = await _mediator.Send(new ModifierNoteCommand(Note), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadListsAsync(ct);
            return Page();
        }

        TempData["Success"] = "Note mise à jour avec succès.";
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync(CancellationToken ct)
    {
        var etudiants = await _mediator.Send(new GetAllEtudiantsQuery(), ct);
        var matieres  = await _mediator.Send(new GetAllMatieresQuery(), ct);
        var typesEval = await _typeEvalRepo.GetAllAsync(ct);
        EtudiantsList = new SelectList(etudiants,  nameof(Etudiant.Id),          nameof(Etudiant.NomComplet));
        MatieresList  = new SelectList(matieres,   nameof(Matiere.Id),           nameof(Matiere.Intitule));
        TypesEvalList = new SelectList(typesEval,  nameof(TypeEvaluationRef.Id), nameof(TypeEvaluationRef.Libelle));
    }
}
